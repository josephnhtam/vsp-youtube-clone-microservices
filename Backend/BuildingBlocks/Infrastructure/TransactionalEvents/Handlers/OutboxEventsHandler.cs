using Domain.TransactionalEvents;
using Domain.TransactionalEvents.Outbox;
using EventBus;
using Infrastructure.TransactionalEvents.Processing;
using Microsoft.Extensions.Logging;

namespace Infrastructure.TransactionalEvents.Handlers {
    public class OutboxEventsHandler : ITransactionalEventsHandler {

        private readonly IEventBus _eventBus;
        private readonly ILogger<OutboxEventsHandler> _logger;

        public OutboxEventsHandler (IEventBus eventBus, ILogger<OutboxEventsHandler> logger) {
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task ProcessTransactionalEventsAsync (List<TransactionalEvent> events, CancellationToken cancellationToken) {
            var deserializedEvents = new List<(IntegrationEventBase @event, IIntegrationEventProperties properties)>();

            foreach (var transactionalEvent in events.Where(x => x.Category == OutboxMessage.Category)) {
                var outboxMessage = transactionalEvent.GetEvent() as OutboxMessage;

                if (outboxMessage == null) {
                    _logger.LogError("Failed to deserialize OutboxMessage.\nType: {Type}\n Data: {Data}"
                        , transactionalEvent.Type, transactionalEvent.Data);
                    throw new InvalidDataException("Failed to deserialize OutboxMessage");
                }

                if (!outboxMessage.DeserializeEvent(out var @event, out var properties)) {
                    _logger.LogError("Failed to deserialize Integration Event.\nOutboxMessage: {Message}"
                        , outboxMessage.ToString());
                    throw new InvalidDataException("Failed to deserialize Integration Event");
                }

                deserializedEvents.Add((@event, properties));
            }

            await _eventBus.PublishEvents(deserializedEvents);
        }

    }
}
