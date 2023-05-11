using Application.FailedResponseHandling;
using Application.FailedResponseHandling.Extensions;
using Duende.IdentityServer;
using HealthChecks.UI.Client;
using IdentityProvider.Configurations;
using Infrastructure.EFCore.Exceptions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using SharedKernel.Exceptions;
using System.Diagnostics;
using System.Reflection;

namespace IdentityProvider {
    internal static class HostExtensions {

        public static WebApplication ConfigureServices (this WebApplicationBuilder builder) {
            RegisterExceptionIdentifiers();

            builder.Services.AddEndpointsApiExplorer()
                            .AddSwaggerGen();

            builder.Services.AddRazorPages();

            builder.AddDbContext()
                   .AddIdentityServer()
                   .AddAuthentication()
                   .AddAuthorization()
                   .AddOpenTelemetry()
                   .AddSerilog()
                   .AddHealthChecks();

            builder.ConfigureForwardedHeaders();

            builder.AddCors()
                   .ConfigureCookiesPolicy();

            builder.Services.AddControllers(options => {
                options.Filters.Add<FailedResponseFilter>();
            }).ConfigureInvalidModelStateResponse();

            return builder.Build();
        }

        public static void ConfigurePipeline (this WebApplication app) {
            app.UseForwardedHeaders();

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment()) {
                app.UseHttpsRedirection();
            }

            app.UseCors();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.MapHealthChecks("/hc", new() {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapRazorPages();

            app.MapControllers();
        }

        private static WebApplicationBuilder ConfigureForwardedHeaders (this WebApplicationBuilder builder) {
            builder.Services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            return builder;
        }

        private static WebApplicationBuilder AddAuthorization (this WebApplicationBuilder builder) {
            builder.Services.AddAuthorization(options => {
                options.AddPolicy("service", builder => {
                    builder.AuthenticationSchemes = new[] { IdentityServerConstants.LocalApi.AuthenticationScheme };
                    builder.RequireAuthenticatedUser();
                    builder.RequireClaim("scope", "vsp_m2m_api");
                });
            });

            return builder;
        }

        private static WebApplicationBuilder AddAuthentication (this WebApplicationBuilder builder) {
            builder.Services.AddAuthentication().AddLocalApi();

            AddGoogle(builder);

            return builder;
        }

        private static void AddGoogle (WebApplicationBuilder builder) {
            string? clientId = builder.Configuration.GetValue<string>("Authentication:Google:ClientId");
            string? clientSecret = builder.Configuration.GetValue<string>("Authentication:Google:ClientSecret");

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret)) {
                builder.Services.AddAuthentication().AddGoogle("Google", options => {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                });
            }
        }

        private static WebApplicationBuilder ConfigureCookiesPolicy (this WebApplicationBuilder builder) {
            if (builder.Environment.IsDevelopment()) {
                // https://stackoverflow.com/questions/50262561/correlation-failed-in-net-core-asp-net-identity-openid-connect
                builder.Services.Configure<CookiePolicyOptions>(options => {
                    options.Secure = CookieSecurePolicy.Always;
                });
            }

            return builder;
        }

        private static WebApplicationBuilder AddIdentityServer (this WebApplicationBuilder builder) {
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var idpBuilder = builder.Services.AddIdentityServer()
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryClients(Config.GetClients(builder.Configuration))
                .AddAspNetIdentity<ApplicationUser>();

            string migrationAssembly = Assembly.GetAssembly(typeof(HostExtensions))!.GetName().Name!;

            idpBuilder.AddOperationalStore(options => {
                options.ConfigureDbContext = ctxOpt =>
                       ctxOpt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"), opt => {
                           opt.EnableRetryOnFailure();
                           opt.MigrationsAssembly(migrationAssembly);
                       });
            });

            idpBuilder.AddDeveloperSigningCredential();

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
                    options.AddService("IdentityProvider");
                })
                .WithTracing(options => {
                    options.AddAspNetCoreInstrumentation()
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

        private static WebApplicationBuilder AddDbContext (this WebApplicationBuilder builder) {
            var configuration = builder.Configuration;

            string migrationAssembly = Assembly.GetAssembly(typeof(HostExtensions))!.GetName().Name!;

            var connectionString = configuration.GetConnectionString("PostgreSQL")!;

            builder.Services.AddDbContext<ApplicationDbContext>(ctx => {
                ctx.UseNpgsql(connectionString, opt => {
                    opt.EnableRetryOnFailure();
                    opt.MigrationsAssembly(migrationAssembly);
                });
            });

            builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

            return builder;
        }

        public static async Task InitializeAsync (this WebApplication app) {
            var config = app.Configuration.GetSection("Initialization");

            if (config.GetValue<bool>("MigrateDatabase")) {
                await app.MigrateDatabases();
            }

            if (config.GetValue<bool>("SeedDatabase")) {
                await app.SeedDatabase();
            }
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

        private static void RegisterExceptionIdentifiers () {
            ExceptionIdentifiers.Register(builder => {
                builder.AddIdentifier(ExceptionCategories.Transient,
                    new TransientHttpExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.Transient,
                    new TransientEFCoreExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.UniqueViolation,
                    new UniqueViolationEFCoreExceptionIdentifier());

                builder.AddIdentifier(ExceptionCategories.ConstraintViolation,
                    new ConstraintViolationEFCoreExceptionIdentifier());
            });
        }

        private static WebApplicationBuilder AddHealthChecks (this WebApplicationBuilder builder) {
            builder.Services.AddHealthChecks();
            return builder;
        }

    }
}
