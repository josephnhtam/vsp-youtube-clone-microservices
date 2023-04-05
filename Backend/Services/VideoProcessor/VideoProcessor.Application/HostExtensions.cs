using Application.DomainEventsDispatching.Extensions;
using Application.Identities.Extensions;
using Application.Identities.Handlers;
using Application.PipelineBehaviours;
using Application.Utilities;
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
using k8s;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using Serilog.Enrichers.Span;
using SharedKernel.Exceptions;
using Storage.Shared.IntegrationEvents;
using System.Diagnostics;
using System.Reflection;
using VideoProcessor.Application.BackgroundTasks;
using VideoProcessor.Application.BackgroundTasks.Processors.FileDownloaders;
using VideoProcessor.Application.BackgroundTasks.Processors.FileUploaders;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoInfoGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoPreviewThumbnailGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoThumbnailGenerators;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.Infrastructure;
using VideoProcessor.Application.Services;
using VideoProcessor.Application.Utilities;
using VideoProcessor.Domain.Contracts;
using VideoProcessor.Infrastructure;
using VideoProcessor.Infrastructure.Repositories;

namespace VideoProcessor.Application {
    public static class HostExtensions {

        public static WebApplication ConfigureServices (this WebApplicationBuilder builder) {
            RegisterExceptionIdentifiers();

            builder.AddConfigurations()
                   .AddHttpClient()
                   .AddDbContext()
                   .AddEventBus()
                   .AddTransactionalEvents()
                   .AddIdempotencyContext()
                   .AddRepository()
                   .AddMediatR()
                   .AddAutoMapper()
                   .AddHttpClient()
                   .AddOpenTelemetry()
                   .AddSerilog()
                   .AddHealthChecks()
                   .AddKubernetesClient()
                   .AddVideoProcessor();

            builder.ConfigureGracefulTermination();

            return builder.Build();
        }

        public static void ConfigurePipeline (this WebApplication app) {
            app.UseHttpsRedirection();

            app.UseMetricServer();

            app.MapHealthChecks("/hc", new() {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
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
                await app.MigrateDatabase<VideoProcessorDbContext>();
            }

            await app.MigrateDatabase<TempDirectoryDbContext>();

            FFmpegHelper.FindFFmpegExecutablesPath();
        }

        private static WebApplicationBuilder AddAutoMapper (this WebApplicationBuilder builder) {
            builder.Services.AddAutoMapper(typeof(HostExtensions).Assembly);

            return builder;
        }

        private static WebApplicationBuilder AddHttpClient (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services.AddBearerTokenHandler(configuration, (clientCredentials) => {
                var section = configuration.GetSection("ClientCredentials");
                section.Bind(clientCredentials);
            });

            builder.Services.AddHttpClient(HttpClients.StorageClient)
                .AddHttpMessageHandler<BearerTokenHandler>()
                .AddTransientHttpErrorPolicy();

            return builder;
        }

        private static WebApplicationBuilder AddVideoProcessor (this WebApplicationBuilder builder) {
            builder.Services
                   .AddTransient<IFileUploader, FileUploader>()
                   .AddTransient<IFileDownloader, FileDownloader>()
                   .AddTransient<IVideoInfoGenerator, VideoInfoGenerator>()
                   .AddTransient<IVideoGenerator, VideoGenerator>()
                   .AddTransient<IVideoThumbnailGenerator, VideoThumbnailGenerator>()
                   .AddTransient<IVideoPreviewThumbnailGenerator, VideoPreviewThumbnailGenerator>()
                   .AddHostedService<VideoProcessingService>();

            return builder;
        }

        private static WebApplicationBuilder AddConfigurations (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services
                .Configure<VideoGeneratorConfiguration>(configuration.GetSection("VideoGeneratorConfiguration"))
                .Configure<VideoProcessorConfiguration>(configuration.GetSection("VideoProcessorConfiguration"))
                .Configure<StorageConfiguration>(configuration.GetSection("StorageConfiguration"))
                .Configure<LocalTempStorageConfiguration>(configuration.GetSection("LocalTempStorageConfiguration"));

            return builder;
        }

        private static WebApplicationBuilder AddMediatR (this WebApplicationBuilder builder) {
            builder.Services.AddMediatR(typeof(HostExtensions).Assembly);

            builder.Services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ActivityPipelineBehaviour<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatingPipelineBehaviour<,>));

            return builder;
        }

        private static WebApplicationBuilder AddDbContext (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            string migrationAssembly = Assembly.GetAssembly(typeof(HostExtensions))!.GetName().Name!;

            var connectionString = configuration.GetConnectionString("PostgreSQL")!;

            builder.Services.AddDbContext<VideoProcessorDbContext>(ctx => {
                ctx.UseNpgsql(connectionString, opt => {
                    opt.MigrationsAssembly(migrationAssembly);
                    opt.EnableRetryOnFailure();
                });
            });

            builder.Services.AddDbContext<TempDirectoryDbContext>(ctx => {
                string dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "tempDir.db");
                ctx.UseSqlite($"Data Source={dbPath}");
            });


            builder.Services.AddHealthChecks().AddDbContextCheck<VideoProcessorDbContext>();

            return builder;
        }

        private static WebApplicationBuilder AddRepository (this WebApplicationBuilder builder) {
            builder.Services
                .AddScoped<IVideoRepository, VideoRepository>()
                .AddScoped<ITempDirectoryRepository, TempDirectoryRepository>()
                .AddDomainEventsAccessor<DomainEventsAccessor<VideoProcessorDbContext>>()
                .AddUnitOfWork<UnitOfWork<VideoProcessorDbContext>>();

            return builder;
        }

        private static WebApplicationBuilder AddIdempotencyContext (this WebApplicationBuilder builder) {
            builder.Services.AddIdempotencyContext<VideoProcessorDbContext>();

            return builder;
        }

        private static WebApplicationBuilder AddTransactionalEvents (this WebApplicationBuilder builder) {
            builder.Services.AddTransactionalEventsContext<TransactionalEventsContext<VideoProcessorDbContext>>(options => {
                options.UseNpgsql();
            });

            builder.Services.AddTransactionalEventsProcessingService()
               .UseEntityFrameworkCore<VideoProcessorDbContext>()
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
                    options.AddService("VideoProcessor");
                })
                .WithTracing(options => {
                    options.AddSource("Mediator", "IntegrationEvent", "VideoProcessor")
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

        private static WebApplicationBuilder AddHealthChecks (this WebApplicationBuilder builder) {
            builder.Services.AddHealthChecks();
            return builder;
        }

        private static WebApplicationBuilder AddKubernetesClient (this WebApplicationBuilder builder) {
            if (KubernetesClientConfiguration.IsInCluster()) {
                var podName = Environment.GetEnvironmentVariable("POD_NAME");
                var podNamespace = Environment.GetEnvironmentVariable("POD_NAMESPACE");

                if (!string.IsNullOrEmpty(podName) && !string.IsNullOrEmpty(podNamespace)) {
                    builder.Services.Configure<KubernetesPodConfiguration>(options => {
                        options.PodName = podName;
                        options.PodNamespace = podNamespace;
                    });

                    builder.Services.AddSingleton(
                        _ => new Kubernetes(KubernetesClientConfiguration.InClusterConfig())
                    );

                    builder.Services.AddSingleton<IKubernetesPodUpdater, KubernetesPodUpdater>();
                }
            }

            return builder;
        }

        private static WebApplicationBuilder ConfigureGracefulTermination (this WebApplicationBuilder builder) {
            var configuration = new GracefulTerminationConfiguration();

            var section = builder.Configuration.GetSection("GracefulTerminationConfiguration");
            section.Bind(configuration);

            builder.Services.Configure<GracefulTerminationConfiguration>(section);

            builder.Services.Configure<HostOptions>(options => {
                options.ShutdownTimeout = TimeSpan.FromMinutes(configuration.ShutdownTimeoutInMinutes);
            });

            return builder;
        }

    }
}
