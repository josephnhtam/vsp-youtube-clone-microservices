using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Infrastructure.Extensions {
    public static class ServicesExtensions {

        public static IServiceCollection AddElasticClient (this IServiceCollection services, ConnectionSettings connectionSettings) {
            services.AddSingleton<IElasticClient>(new ElasticClient(connectionSettings.ThrowExceptions(true)));
            return services;
        }

    }
}
