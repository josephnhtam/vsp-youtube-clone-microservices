using Application.DomainEventsDispatching.Extensions;
using Application.FailedResponseHandling;
using Application.FailedResponseHandling.Extensions;
using Application.PipelineBehaviours;
using EventBus.RabbitMQ.Exceptions;
using EventBus.RabbitMQ.Extensions;
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
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using SharedKernel.Exceptions;
using Storage.API.Application.BackgroundTasks;
using Storage.API.Application.Configurations;
using Storage.API.Application.Services;
using Storage.Domain.Contracts;
using Storage.Infrastructure;
using Storage.Infrastructure.LocalStorage;
using Storage.Infrastructure.Repositories;
using Storage.Shared.IntegrationEvents;
using System.Diagnostics;
using System.Reflection;

namespace Storage {
    public static class HostExtensions {

        public static WebApplication ConfigureServices (this WebApplicationBuilder builder) {
            RegisterExceptionIdentifiers();

            builder.Services.AddEndpointsApiExplorer()
                            .AddSwaggerGen();

            builder.AddConfigurations()
                   .AddDbContext()
                   .AddRepository()
                   .AddTransactionalEvents()
                   .AddIdempotencyContext()
                   .AddEventBus()
                   .AddServices()
                   .AddBackgroundService()
                   .AddMediatR()
                   .AddOpenTelemetry()
                   .AddSerilog()
                   .AddHealthChecks();

            builder.AddLocalStorageRepository();

            builder.AddAuthentication()
                   .AddAuthorization()
                   .AddCors();

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

            if (!app.Environment.IsDevelopment()) {
                app.UseHttpsRedirection();
            }

            app.UseCors();

            app.UseLocalStorageStaticFiles();

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

        private static WebApplicationBuilder AddCors (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services.AddCors(options => {
                options.AddDefaultPolicy(policy => {
                    policy.WithOrigins(configuration.GetSection("Cors:Origins").Get<string[]>()!)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

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
                    options.AddService("Storage");
                })
                .WithTracing(options => {
                    options.AddSource("Mediator", "IntegrationEvent", "Storage")
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

        private static WebApplicationBuilder AddServices (this WebApplicationBuilder builder) {
            builder.Services
                .AddScoped<IFileStorageHandler, FileStorageHandler>()
                .AddSingleton<IImageFormatChecker, ImageFormatChecker>();

            return builder;
        }
        private static WebApplicationBuilder AddBackgroundService (this WebApplicationBuilder builder) {

            builder.Services.AddHostedService<CleanupService>();

            return builder;
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

        public static async Task InitializeAsync (this WebApplication app) {
            var config = app.Configuration.GetSection("Initialization");

            if (config.GetValue<bool>("MigrateDatabase")) {
                await app.MigrateDatabase<StorageDbContext>();
            }
        }

        private static WebApplicationBuilder AddConfigurations (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services
                .Configure<VideoUploadTokenValidationConfiguration>(configuration.GetSection("VideoUploadTokenValidationConfiguration"))
                .Configure<VideoStorageConfiguration>(configuration.GetSection("VideoStorageConfiguration"))
                .Configure<ImageUploadTokenValidationConfiguration>(configuration.GetSection("ImageUploadTokenValidationConfiguration"))
                .Configure<ImageStorageConfiguration>(configuration.GetSection("ImageStorageConfiguration"))
                .Configure<CleanupConfiguration>(configuration.GetSection("CleanupConfiguration"));

            return builder;
        }

        private static WebApplicationBuilder AddMediatR (this WebApplicationBuilder builder) {
            builder.Services.AddMediatR(typeof(HostExtensions).Assembly);

            builder.Services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ActivityPipelineBehaviour<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatingPipelineBehaviour<,>));

            return builder;
        }

        private static void UseLocalStorageStaticFiles (this WebApplication app) {
            var localStorageConfig = app.Services.GetRequiredService<IOptions<LocalStorageConfiguration>>().Value;

            if (localStorageConfig.ServeStaticFiles) {
                string localStorageRootPath = LocalStorageHelper.GetRootPath(localStorageConfig);

                if (!Directory.Exists(localStorageRootPath)) {
                    Directory.CreateDirectory(localStorageRootPath);
                }

                var contentTypeProvider = new FileExtensionContentTypeProvider();
                contentTypeProvider.Mappings[".mkv"] = "video/x-matroska";
                contentTypeProvider.Mappings[".wmv"] = "video/x-ms-wmv";
                contentTypeProvider.Mappings[".ogg"] = "video/ogg";
                contentTypeProvider.Mappings[".rmvb"] = "application/vnd.rn-realmedia-vbr";

                app.UseStaticFiles(new StaticFileOptions {
                    ContentTypeProvider = contentTypeProvider,
                    FileProvider = new PhysicalFileProvider(localStorageRootPath),
                    RequestPath = localStorageConfig.RequestPath,

                    OnPrepareResponse = ctx => {
                        var corsService = app.Services.GetRequiredService<ICorsService>();
                        var corsPolicyProvider = app.Services.GetRequiredService<ICorsPolicyProvider>();

                        if (corsService == null || corsPolicyProvider == null) return;

                        var policy = corsPolicyProvider
                            .GetPolicyAsync(ctx.Context, null)
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        if (policy == null) return;

                        var corsResult = corsService.EvaluatePolicy(ctx.Context, policy);

                        corsService.ApplyResult(corsResult, ctx.Context.Response);

                        //ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                        //ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                        //    "Origin, X-Requested-With, Content-Type, Accept");
                    },
                });
            }
        }

        private static WebApplicationBuilder AddDbContext (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            string migrationAssembly = Assembly.GetAssembly(typeof(HostExtensions))!.GetName().Name!;

            var connectionString = configuration.GetConnectionString("PostgreSQL")!;

            builder.Services.AddDbContext<StorageDbContext>(ctx => {
                ctx.UseNpgsql(connectionString, opt => {
                    opt.MigrationsAssembly(migrationAssembly);
                    opt.EnableRetryOnFailure();
                });
            });

            builder.Services.AddHealthChecks().AddDbContextCheck<StorageDbContext>();

            return builder;
        }

        private static WebApplicationBuilder AddRepository (this WebApplicationBuilder builder) {
            builder.Services
                .AddScoped<IFileRepository, FileRepository>()
                .AddScoped<IFileTrackingRepository, FileTrackingRepository>()
                .AddDomainEventsAccessor<DomainEventsAccessor<StorageDbContext>>()
                .AddUnitOfWork<UnitOfWork<StorageDbContext>>();

            return builder;
        }

        private static WebApplicationBuilder AddIdempotencyContext (this WebApplicationBuilder builder) {
            builder.Services.AddIdempotencyContext<StorageDbContext>();

            return builder;
        }

        private static WebApplicationBuilder AddTransactionalEvents (this WebApplicationBuilder builder) {
            builder.Services.AddTransactionalEventsContext<TransactionalEventsContext<StorageDbContext>>(options => {
                options.UseNpgsql();
            });

            builder.Services.AddTransactionalEventsProcessingService()
               .UseEntityFrameworkCore<StorageDbContext>()
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

        private static WebApplicationBuilder AddLocalStorageRepository (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services
                .AddScoped<IStorageRepository, LocalStorageRepository>()
                .Configure<LocalStorageConfiguration>(configuration.GetSection("LocalStorageConfiguration"));

            return builder;
        }

        private static WebApplicationBuilder AddHealthChecks (this WebApplicationBuilder builder) {
            builder.Services.AddHealthChecks();
            return builder;
        }

    }
}
