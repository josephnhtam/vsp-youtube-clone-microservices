using Infrastructure.Caching.Layers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Caching.Extensions {
    public static class ServicesExtensions {

        public static IServiceCollection AddCacheContext (this IServiceCollection services) {
            services.TryAddScoped<ICacheContext, CacheContext>();
            return services;
        }

        public static IServiceCollection AddInMemoryCachingLayer (this IServiceCollection services, Action<MemoryCacheOptions>? configureMemoryCache = null, Action<InMemoryCacheLayer>? configureLayer = null) {
            services.AddMemoryCache();
            services.AddScoped<ICachingLayer, InMemoryCacheLayer>();

            if (configureMemoryCache != null) {
                services.Configure(configureMemoryCache);
            }

            if (configureLayer != null) {
                services.Configure(configureLayer);
            }

            return services;
        }

        public static IServiceCollection AddRedisCachingLayer (this IServiceCollection services, Action<RedisCacheLayer>? configureLayer = null) {
            services.AddScoped<ICachingLayer, RedisCacheLayer>();

            if (configureLayer != null) {
                services.Configure(configureLayer);
            }

            return services;
        }

    }
}
