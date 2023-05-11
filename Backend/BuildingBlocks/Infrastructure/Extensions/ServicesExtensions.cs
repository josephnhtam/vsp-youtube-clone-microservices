using Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Extensions {
    public static class ServicesExtensions {

        public static IServiceCollection AddUnitOfWork<T> (this IServiceCollection services)
            where T : class, IUnitOfWork {
            services.TryAddScoped<IUnitOfWork, T>();
            return services;
        }

    }
}
