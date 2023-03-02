using Application.Identities.Configurations;
using Application.Identities.Handlers;
using Application.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Identities.Extensions {
    public static class ServicesExtensions {

        public static IServiceCollection AddBearerTokenHandler (this IServiceCollection services, IConfiguration configuration, Action<ClientCredentials> configureClientCredentials) {
            services
                .Configure<ClientCredentials>(configureClientCredentials)
                .AddTransient<BearerTokenHandler>()
                .AddMemoryCache();

            services.AddHttpClient("BearerTokenHandlerClient")
                    .AddTransientHttpErrorPolicy();

            return services;
        }

    }
}
