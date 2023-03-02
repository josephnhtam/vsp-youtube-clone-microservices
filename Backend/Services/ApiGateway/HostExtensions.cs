using ApiGateway.Configurations;
using HealthChecks.UI.Client;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using System.Diagnostics;
using System.Threading.RateLimiting;

namespace ApiGateway {
    public static class HostExtensions {

        public static WebApplication ConfigureServices (this WebApplicationBuilder builder) {
            builder.AddReverseProxy()
                   .AddRateLimiter()
                   .AddOpenTelemetry()
                   .AddSerilog()
                   .AddHealthChecks();

            builder.AddCors();

            return builder.Build();
        }

        public static void ConfigurePipeline (this WebApplication app) {
            app.UseHttpsRedirection();

            app.UseCors();

            app.UseRateLimiter();

            app.MapHealthChecks("/hc", new() {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapReverseProxy();
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
                    options.AddService("ApiGateway");
                })
                .WithTracing(options => {
                    options.AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddJaegerExporter(options => {
                               configuration.GetSection("JaegerExporterOptions").Bind(options);
                           });
                });

            return builder;
        }

        private static WebApplicationBuilder AddRateLimiter (this WebApplicationBuilder builder) {
            builder.Services.AddRateLimiter(options => {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context => {
                    return RateLimitPartition.GetFixedWindowLimiter(context.Connection.Id, key => {
                        return new FixedWindowRateLimiterOptions {
                            Window = TimeSpan.FromSeconds(3),
                            PermitLimit = 32,
                            QueueLimit = 256,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        };
                    });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return builder;
        }

        private static WebApplicationBuilder AddReverseProxy (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services.AddReverseProxy()
                            .LoadFromMemory(
                                Config.GetRoutes(configuration),
                                Config.GetClusters(configuration)
                            );

            return builder;
        }

        private static WebApplicationBuilder AddCors (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            builder.Services.AddCors(options => {
                options.AddPolicy("CorsPolicy", policy => {
                    policy.WithOrigins(configuration.GetSection("Cors:Origins").Get<string[]>()!)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });

                options.AddPolicy("HubCorsPolicy", policy => {
                    policy.WithOrigins(configuration.GetSection("Cors:Origins").Get<string[]>()!)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return builder;
        }

        private static WebApplicationBuilder AddHealthChecks (this WebApplicationBuilder builder) {
            builder.Services.AddHealthChecks();
            return builder;
        }

    }
}
