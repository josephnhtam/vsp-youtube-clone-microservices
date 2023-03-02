using Application.DomainEventsDispatching.Extensions;
using Application.FailedResponseHandling;
using Application.FailedResponseHandling.Extensions;
using Application.PipelineBehaviours;
using EventBus.RabbitMQ.Exceptions;
using EventBus.RabbitMQ.Extensions;
using FluentValidation;
using HealthChecks.UI.Client;
using Infrastructure.EFCore;
using Infrastructure.EFCore.DomainEventsDispatching;
using Infrastructure.EFCore.Exceptions;
using Infrastructure.EFCore.Idempotency.Extensions;
using Infrastructure.EFCore.TransactionalEvents;
using Infrastructure.EFCore.TransactionalEvents.Extensions;
using Infrastructure.EFCore.TransactionalEvents.Processing.Extensions;
using Infrastructure.Extensions;
using Infrastructure.TransactionalEvents.Extensions;
using Infrastructure.TransactionalEvents.Handlers;
using Infrastructure.TransactionalEvents.Processing.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using SharedKernel.Exceptions;
using Storage.Shared.IntegrationEvents;
using System.Diagnostics;
using System.Reflection;
using VideoStore.API.Application.Configurations;
using VideoStore.Domain.Contracts;
using VideoStore.Infrastructure;
using VideoStore.Infrastructure.Repositories;

namespace VideoStore.API {
    public static class HostExtensions {

        public static WebApplication ConfigureServices (this WebApplicationBuilder builder) {
            RegisterExceptionIdentifiers();

            builder.Services.AddEndpointsApiExplorer()
                            .AddSwaggerGen();

            builder.AddConfigurations()
                   .AddDbContext()
                   .AddEventBus()
                   .AddTransactionalEvents()
                   .AddIdempotencyContext()
                   .AddRepositories()
                   .AddMediatR()
                   .AddAutoMapper()
                   .AddFluentValidators()
                   .AddOpenTelemetry()
                   .AddSerilog()
                   .AddHealthChecks();

            builder.AddAuthentication()
                   .AddAuthorization();

            builder.Services.AddControllers(options => {
                options.Filters.Add<FailedResponseFilter>();
            }).ConfigureInvalidModelStateResponse();

            return builder.Build();
        }

        public static void ConfigurePipeline (this WebApplication app) {
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapHealthChecks("/hc", new() {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapControllers();
        }

        private static void RegisterExceptionIdentifiers () {
            ExceptionIdentifiers.Register(builder => {
                builder.AddIdentifier(ExceptionCategories.Transient,
                    new TransientHttpExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.Transient,
                   new TransientRabbitMQExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.Transient,
                    new TransientEFCoreExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.UniqueViolation,
                    new UniqueViolationEFCoreExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.ConstraintViolation,
                    new ConstraintViolationEFCoreExceptionIdentifier());
            });
        }

        public static async Task InitializeAsync (this WebApplication app) {
            var config = app.Configuration.GetSection("Initialization");

            if (config.GetValue<bool>("MigrateDatabase")) {
                await app.MigrateDatabase<VideoStoreDbContext>();
            }
        }

        private static WebApplicationBuilder AddAuthentication (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.Authority = configuration.GetValue<string>("Urls:IdentityProvider");
                    options.Audience = "vsp_resource";
                    options.RefreshInterval = TimeSpan.FromSeconds(30);

                    if (builder.Environment.IsDevelopment()) {
                        options.RequireHttpsMetadata = false;

                        options.TokenValidationParameters = new TokenValidationParameters {
                            ValidateIssuer = false
                        };
                    }
                });

            return builder;
        }

        private static WebApplicationBuilder AddAuthorization (this WebApplicationBuilder builder) {
            builder.Services.AddAuthorization(options => {
                options.AddPolicy("user", builder => {
                    builder.RequireAuthenticatedUser();
                    builder.RequireClaim("scope", "vsp_api");
                });

                options.AddPolicy("service", builder => {
                    builder.RequireAuthenticatedUser();
                    builder.RequireClaim("scope", "vsp_m2m_api");
                });
            });

            return builder;
        }

        private static WebApplicationBuilder AddConfigurations (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services
                .Configure<StorageConfiguration>(configuration.GetSection("StorageConfiguration"));

            return builder;
        }

        private static WebApplicationBuilder AddSerilog (this WebApplicationBuilder builder) {
            builder.Host.UseSerilog((context, config) => {
                var logstashUri = context.Configuration.GetValue<string>("Logstash:Uri")!;

                config.Enrich.FromLogContext()
                      .Enrich.WithMachineName()
                      .Enrich.WithEnvironmentName()
                      .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                      .Enrich.WithSpan(new SpanOptions {
                          IncludeOperationName = true,
                          IncludeTags = true
                      })
                      .ReadFrom.Configuration(context.Configuration)
                      .WriteTo.Debug()
                      .WriteTo.Console()
                      .WriteTo.DurableHttpUsingFileSizeRolledBuffers(logstashUri, "Logs-Buffer");
            });

            return builder;
        }

        private static WebApplicationBuilder AddOpenTelemetry (this WebApplicationBuilder builder) {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;

            var configuration = builder.Configuration;

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(options => {
                    options.AddService("VideoStore");
                })
                .WithTracing(options => {
                    options.AddSource("Mediator", "IntegrationEvent", "VideoStore")
                           .AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddEntityFrameworkCoreInstrumentation(options => {
                               options.SetDbStatementForText = true;
                           })
                           .AddJaegerExporter(options => {
                               configuration.GetSection("JaegerExporterOptions").Bind(options);
                           });
                });

            return builder;
        }

        private static WebApplicationBuilder AddMediatR (this WebApplicationBuilder builder) {
            builder.Services.AddMediatR(typeof(HostExtensions).Assembly);

            builder.Services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ActivityPipelineBehaviour<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatingPipelineBehaviour<,>));

            return builder;
        }

        private static WebApplicationBuilder AddAutoMapper (this WebApplicationBuilder builder) {
            builder.Services.AddAutoMapper(typeof(HostExtensions).Assembly);

            return builder;
        }

        private static WebApplicationBuilder AddFluentValidators (this WebApplicationBuilder builder) {
            builder.Services.AddValidatorsFromAssembly(typeof(HostExtensions).Assembly);

            return builder;
        }

        private static WebApplicationBuilder AddDbContext (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            string migrationAssembly = Assembly.GetAssembly(typeof(HostExtensions))!.GetName().Name!;

            var connectionString = configuration.GetConnectionString("PostgreSQL")!;

            builder.Services.AddDbContext<VideoStoreDbContext>(ctx => {
                ctx.UseNpgsql(connectionString, opt => {
                    opt.MigrationsAssembly(migrationAssembly);
                    opt.EnableRetryOnFailure();
                });
            });

            builder.Services.AddHealthChecks().AddDbContextCheck<VideoStoreDbContext>();

            return builder;
        }

        private static WebApplicationBuilder AddRepositories (this WebApplicationBuilder builder) {
            builder.Services
                .AddScoped<IVideoRepository, VideoRepository>()
                .AddScoped<IUserProfileRepository, UserProfileRepository>()
                .AddDomainEventsAccessor<DomainEventsAccessor<VideoStoreDbContext>>()
                .AddUnitOfWork<UnitOfWork<VideoStoreDbContext>>();

            return builder;
        }

        private static WebApplicationBuilder AddIdempotencyContext (this WebApplicationBuilder builder) {
            builder.Services.AddIdempotencyContext<VideoStoreDbContext>();

            return builder;
        }

        private static WebApplicationBuilder AddTransactionalEvents (this WebApplicationBuilder builder) {
            builder.Services.AddTransactionalEventsContext<TransactionalEventsContext<VideoStoreDbContext>>(options => {
                options.UseNpgsql();
            });

            builder.Services.AddTransactionalEventsProcessingService()
               .UseEntityFrameworkCore<VideoStoreDbContext>()
               .AddEventsHandler<OutboxEventsHandler>();

            return builder;
        }

        private static WebApplicationBuilder AddEventBus (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            var eventBusBuilder = builder.Services.AddRabbitMQEventBus((config) => {
                config.ConfigureConnectionFactory((cf) => {
                    cf.ConsumerDispatchConcurrency = 32 * Environment.ProcessorCount;
                    cf.HostName = configuration.GetValue<string>("RabbitMQConfiguration:HostName");
                    cf.UserName = configuration.GetValue<string>("RabbitMQConfiguration:UserName");
                    cf.Password = configuration.GetValue<string>("RabbitMQConfiguration:Password");
                })
                .AddHealthCheck(failureStatus: HealthStatus.Degraded);
            }).AddIntegrationEvents(Assembly.GetExecutingAssembly(), typeof(FilesCleanupIntegrationEvent).Assembly)
              .AddIntegrationEventHandlers(Assembly.GetExecutingAssembly());

            return builder;
        }

        private static WebApplicationBuilder AddHealthChecks (this WebApplicationBuilder builder) {
            builder.Services.AddHealthChecks();
            return builder;
        }

    }
}
