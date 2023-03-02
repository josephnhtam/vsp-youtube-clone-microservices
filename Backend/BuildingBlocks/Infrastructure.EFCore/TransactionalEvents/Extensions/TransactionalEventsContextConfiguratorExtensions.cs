using Infrastructure.TransactionalEvents.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EFCore.TransactionalEvents.Extensions {
    public static class TransactionalEventsContextConfiguratorExtensions {

        public static void UseNpgsql (this TransactionalEventsContextConfigurator configurator) {
            configurator.Services.Configure<TransactionalEventsContextConfig>(x => {
                x.CommandResolver = new NpgsqlTransactionalEventsCommandResolver();
            });
        }

    }
}
