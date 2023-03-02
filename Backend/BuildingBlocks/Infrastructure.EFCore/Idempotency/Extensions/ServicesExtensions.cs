using Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.EFCore.Idempotency.Extensions {
    public static class ServicesExtensions {

        public static IServiceCollection AddIdempotencyContext<TDbContext> (this IServiceCollection services)
            where TDbContext : DbContext {
            services.TryAddScoped<IIdempotencyContext, IdempotencyContext<TDbContext>>();
            return services;
        }

    }
}
