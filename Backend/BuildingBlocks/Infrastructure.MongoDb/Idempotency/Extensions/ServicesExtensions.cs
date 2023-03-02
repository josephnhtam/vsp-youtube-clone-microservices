using Infrastructure.Idempotency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.MongoDb.Idempotency.Extensions {
    public static class ServicesExtensions {

        public static IServiceCollection AddIdempotencyContext (this IServiceCollection services) {
            services.TryAddScoped<IIdempotencyContext, IdempotencyContext>();
            return services;
        }

    }
}
