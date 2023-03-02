using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.TransactionalEvents.Processing.Extensions {
    public interface ITransactionalEventsProcessorBuilder {
        IServiceCollection Services { get; }
        ITransactionalEventsProcessorBuilder AddEventsHandler<TEventHandler> ()
            where TEventHandler : class, ITransactionalEventsHandler;
        ITransactionalEventsProcessorBuilder AddEventsHandler<TEventHandler> (Func<IServiceProvider, TEventHandler> factory)
            where TEventHandler : class, ITransactionalEventsHandler;
    }

    public class TransactionalEventsProcessorBuilder : ITransactionalEventsProcessorBuilder {
        public IServiceCollection Services { get; private set; }

        public TransactionalEventsProcessorBuilder (IServiceCollection services) {
            Services = services;
        }

        public ITransactionalEventsProcessorBuilder AddEventsHandler<TEventHandler> ()
            where TEventHandler : class, ITransactionalEventsHandler {
            Services.AddTransient<ITransactionalEventsHandler, TEventHandler>();
            return this;
        }

        public ITransactionalEventsProcessorBuilder AddEventsHandler<TEventHandler> (Func<IServiceProvider, TEventHandler> factory)
            where TEventHandler : class, ITransactionalEventsHandler {
            Services.AddTransient<ITransactionalEventsHandler, TEventHandler>(factory);
            return this;
        }
    }
}
