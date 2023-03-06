using Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Application.DomainEventsDispatching.Extensions {
    public static class ServicesExtensions {
        public static IServiceCollection AddDomainEventsAccessor<TEventAccessor> (this IServiceCollection services)
            where TEventAccessor : class, IDomainEventsAccessor {
            services.TryAddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
            services.TryAddScoped<IDomainEventsAccessor, TEventAccessor>();

            return services;
        }
    }
}
