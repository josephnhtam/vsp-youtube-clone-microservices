using Domain.TransactionalEvents.Contracts;
using EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.TransactionalEvents.Outbox {
    public static class TransactionalEventsContextExtensions {

        private static Type? _propertiesType;

        public static void AddOutboxMessage (
           this ITransactionalEventsContext context,
           IntegrationEventBase @event,
           Action<IIntegrationEventProperties>? propertiesConfigurator = null) {

            var transactionalEvent = CreateEvent(context, @event, propertiesConfigurator);

            context.AddEvent(transactionalEvent);
        }

        public static void AddOutboxMessage (
           this ITransactionalEventsContext context,
           string? groupId,
           IntegrationEventBase @event,
           Action<IIntegrationEventProperties>? propertiesConfigurator = null) {

            var transactionalEvent = CreateEvent(context, @event, propertiesConfigurator);

            context.AddEvent(groupId, transactionalEvent);
        }

        private static TransactionalEvent CreateEvent (ITransactionalEventsContext context, IntegrationEventBase @event, Action<IIntegrationEventProperties>? propertiesConfigurator) {
            if (_propertiesType == null) {
                var eventBus = context.ServiceProvider.GetRequiredService<IEventBus>();
                if (eventBus == null) {
                    throw new ArgumentNullException("Event bus is not added");
                }

                Interlocked.CompareExchange(ref _propertiesType, eventBus.GetIntegrationEventPropertiesType(), null);
            }

            IIntegrationEventProperties properties = (Activator.CreateInstance(_propertiesType) as IIntegrationEventProperties)!;
            propertiesConfigurator?.Invoke(properties);

            var outboxMessage = new OutboxMessage(@event, properties);

            var transactionalEvent = new TransactionalEvent(OutboxMessage.Category, outboxMessage);
            return transactionalEvent;
        }
    }
}
