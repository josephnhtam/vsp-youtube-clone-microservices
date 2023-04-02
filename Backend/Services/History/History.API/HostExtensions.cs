using Application.DomainEventsDispatching.Extensions;
using Application.FailedResponseHandling;
using Application.FailedResponseHandling.Extensions;
using Application.PipelineBehaviours;
using EventBus.RabbitMQ.Exceptions;
using EventBus.RabbitMQ.Extensions;
using FluentValidation;
using HealthChecks.UI.Client;
using History.API.Application.BackgroundTasks;
using History.API.Application.Configurations;
using History.Domain.Contracts;
using History.Domain.Models;
using History.Infrastructure.ClassMaps;
using History.Infrastructure.CollectionSeeders;
using History.Infrastructure.Configurations;
using History.Infrastructure.Contracts;
using History.Infrastructure.IndexCreators;
using History.Infrastructure.Managers;
using History.Infrastructure.Repositories;
using History.Infrastructure.Services;
using Infrastructure.Caching.Extensions;
using Infrastructure.Elasticsearch;
using Infrastructure.Extensions;
using Infrastructure.MongoDb;
using Infrastructure.MongoDb.DomainEventsDispatching;
using Infrastructure.MongoDb.Exceptions;
using Infrastructure.MongoDb.Extensions;
using Infrastructure.MongoDb.Idempotency.Extensions;
using Infrastructure.MongoDb.TransactionalEvents;
using Infrastructure.MongoDb.TransactionalEvents.Processing.Extensions;
using Infrastructure.TransactionalEvents.Extensions;
using Infrastructure.TransactionalEvents.Handlers;
using Infrastructure.TransactionalEvents.Processing.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using SharedKernel.Exceptions;
using StackExchange.Redis;
using System.Diagnostics;
using System.Reflection;

namespace History.API {
    public static class HostExtensions {

        public static WebApplication ConfigureServices (this WebApplicationBuilder builder) {
            RegisterExceptionIdentifiers();

            builder.Services.AddEndpointsApiExplorer()
                            .AddSwaggerGen();

            builder.Services.AddHttpContextAccessor();

            builder.AddConfigurations()
                   .AddCaching()
                   .AddDbContext()
                   .AddEventBus()
                   .AddTransactionalEvents()
                   .AddIdempotencyContext()
                   .AddRepositories()
                   .AddMediatR()
                   .AddAutoMapper()
                   .AddFluentValidators()
                   .AddServices()
                   .AddBackgroundServices()
                   .AddRedis()
                   .AddElasticClient()
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
                    new TransientMongoExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.UniqueViolation,
                    new UniqueViolationMongoExceptionIdentifier());
            });
        }

        public static async Task InitializeAsync (this WebApplication app) {
            var config = app.Configuration.GetSection("Initialization");

            app.RegisterMongoClassMaps(typeof(VideoClassMap).Assembly);

            if (config.GetValue<bool>("MigrateDatabase")) {
                await app.SeedMongoCollections(typeof(VideoCollectionSeeder).Assembly);
            }

            if (config.GetValue<bool>("CreateIndices")) {
                await app.CreateElasticsearchIndices(typeof(UserVideoHistoryIndexCreator).Assembly);
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

        private static WebApplicationBuilder AddCaching (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services.AddSingleton<ICacheKeyProvider, CacheKeyProvider>()
                            .AddCacheContext();

            builder.Services.AddRedisCachingLayer();

            return builder;
        }

        private static WebApplicationBuilder AddConfigurations (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services
                .Configure<StorageConfiguration>(configuration.GetSection("StorageConfiguration"))
                .Configure<MetricsSyncConfiguration>(configuration.GetSection("MetricsSyncConfiguration"))
                .Configure<CachingConfigurations>(configuration.GetSection("CachingConfigurations"));

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
                    options.AddService("History");
                })
                .WithTracing(options => {
                    options.AddSource("Mediator", "IntegrationEvent", "History", "CacheContext")
                           .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                           .AddRedisInstrumentation(null, options => {
                               options.SetVerboseDatabaseStatements = true;
                           })
                           .AddElasticsearchClientInstrumentation(options => {
                               options.ParseAndFormatRequest = true;
                           })
                           .AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddJaegerExporter(options => {
                               configuration.GetSection("JaegerExporterOptions").Bind(options);
                           });
                });

            return builder;
        }

        private static WebApplicationBuilder AddRedis (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            var connectionString = configuration.GetConnectionString("Redis")!;

            builder.Services.AddSingleton<IConnectionMultiplexer>((_) => {
                return ConnectionMultiplexer.Connect(connectionString);
            });

            builder.Services.AddHealthChecks().AddRedis(connectionString);

            return builder;
        }

        private static WebApplicationBuilder AddServices (this WebApplicationBuilder builder) {
            builder.Services
                .AddScoped<IUserHistoryCommandManager, UserHistoryCommandManager>()
                .AddScoped<IUserHistoryQueryManager, UserHistoryQueryManager>();

            return builder;
        }

        private static WebApplicationBuilder AddBackgroundServices (this WebApplicationBuilder builder) {
            builder.Services.AddHostedService<MetricsSyncBackgroundService>();

            return builder;
        }

        private static WebApplicationBuilder AddMediatR (this WebApplicationBuilder builder) {
            builder.Services.AddMediatR(typeof(HostExtensions).Assembly, typeof(VideoRepository).Assembly);

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

            builder.Services.AddMongoDb((options) => {
                var settings = MongoClientSettings.FromConnectionString(configuration.GetConnectionString("MongoDb"));
                settings.RetryReads = true;
                settings.RetryWrites = true;

                settings.ClusterConfigurator = cb =>
                    cb.Subscribe(new DiagnosticsActivityEventSubscriber(new InstrumentationOptions {
                        CaptureCommandText = true,
                    }));

                options.ConfigureMongoClientSettings(settings)
                       .AddHealthCheck();
            }).AddCollection<UserProfile>("history_manager", "user_profile")
              .AddCollection<Video>("history_manager", "video")
              .AddTransactionalEventsCollection("history_manager")
              .AddIdempotentOperationsCollection("history_manager");

            return builder;
        }

        private static WebApplicationBuilder AddRepositories (this WebApplicationBuilder builder) {
            builder.Services
                .AddScoped<IVideoRepository, VideoRepository>()
                .AddScoped<IUserProfileRepository, UserProfileRepository>()
                .AddDomainEventEmittersTracker()
                .AddDomainEventsAccessor<DomainEventsAccessor>()
                .AddUnitOfWork<UnitOfWork>();

            builder.Services
                .AddScoped<ICachedVideoRepository, CachedVideoRepository>()
                .AddScoped<ICachedUserProfileRepository, CachedUserProfileRepository>();

            return builder;
        }

        private static WebApplicationBuilder AddIdempotencyContext (this WebApplicationBuilder builder) {
            builder.Services.AddIdempotencyContext();

            return builder;
        }

        private static WebApplicationBuilder AddTransactionalEvents (this WebApplicationBuilder builder) {
            builder.Services.AddTransactionalEventsContext<TransactionalEventsContext>();

            builder.Services.AddTransactionalEventsProcessingService()
               .UseMongoDb()
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
            }).AddIntegrationEvents(Assembly.GetExecutingAssembly())
              .AddIntegrationEventHandlers(Assembly.GetExecutingAssembly());

            return builder;
        }

        private static WebApplicationBuilder AddElasticClient (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            var uri = new Uri(configuration.GetValue<string>("ElasticsearchConfiguration:Uri")!);
            var connectionSettings = new Nest.ConnectionSettings(uri);

            builder.Services.AddElasticClient(connectionSettings);

            builder.Services.AddHealthChecks().AddElasticsearch(uri.AbsoluteUri);

            return builder;
        }

        private static WebApplicationBuilder AddHealthChecks (this WebApplicationBuilder builder) {
            builder.Services.AddHealthChecks();
            return builder;
        }

    }
}
