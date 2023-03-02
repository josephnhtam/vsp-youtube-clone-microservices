using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.TransactionalEvents.Extensions {
    public class TransactionalEventsContextConfigurator {

        public IServiceCollection Services { get; private set; }

        public TransactionalEventsContextConfigurator (IServiceCollection services) {
            Services = services;
        }

    }
}
