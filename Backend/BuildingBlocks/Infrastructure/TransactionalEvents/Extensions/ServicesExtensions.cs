using Domain.TransactionalEvents.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.TransactionalEvents.Extensions {
    public static class ServicesExtensions {

        public static IServiceCollection AddTransactionalEventsContext<TContext>
            (this IServiceCollection services, Action<TransactionalEventsContextConfigurator>? options = null)
            where TContext : class, ITransactionalEventsContext, ITransactionalEventsCommitter {
            services.TryAddScoped<ITransactionalEventsContext, TContext>();

            services.TryAddScoped<ITransactionalEventsCommitter>(sp => {
                var committer = sp.GetRequiredService<ITransactionalEventsContext>() as ITransactionalEventsCommitter;
                return committer!;
            });

            if (options != null) {
                TransactionalEventsContextConfigurator configurator = new TransactionalEventsContextConfigurator(services);
                options.Invoke(configurator);
            }

            return services;
        }

    }
}
