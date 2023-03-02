using System.Threading.Channels;

namespace EventBus.RabbitMQ {
    public class RabbitMQEventBus : IPendingEvents, IEventBus, IDeadLetterEventBus {

        private Channel<IPendingEvent> _pendingEvents;

        public RabbitMQEventBus () {
            _pendingEvents = Channel.CreateUnbounded<IPendingEvent>(new UnboundedChannelOptions {
                AllowSynchronousContinuations = true
            });
        }

        public Type GetIntegrationEventPropertiesType () {
            return typeof(RabbitMQIntegrationEventProperties);
        }

        public async Task PublishDeadLetterEvent (string deadLetterQueue, ReadOnlyMemory<byte> body, Exception failure) {
            var pendingEvent = new PendingDeadLetterEvent(deadLetterQueue, body, failure);
            await _pendingEvents.Writer.WriteAsync(pendingEvent);
            await pendingEvent.WaitAsync();
        }

        public async Task PublishEvent (IntegrationEventBase @event, Action<IIntegrationEventProperties>? propertiesConfigurator = null) {
            var properties = new RabbitMQIntegrationEventProperties();
            propertiesConfigurator?.Invoke(properties);

            var pendingEvent = new PendingEvent(@event.GetType(), @event, properties);
            await _pendingEvents.Writer.WriteAsync(pendingEvent);
            await pendingEvent.WaitAsync();
        }

        public async Task PublishEvent (IntegrationEventBase @event, IIntegrationEventProperties properties) {
            var rabbitMQProperties = properties as RabbitMQIntegrationEventProperties;

            if (rabbitMQProperties == null) {
                throw new ArgumentException($"The type of the properties must be {nameof(RabbitMQIntegrationEventProperties)}");
            }

            var pendingEvent = new PendingEvent(@event.GetType(), @event, rabbitMQProperties);
            await _pendingEvents.Writer.WriteAsync(pendingEvent);
            await pendingEvent.WaitAsync();
        }

        public async Task PublishEvents (IEnumerable<(IntegrationEventBase, IIntegrationEventProperties)> events) {
            List<Task> confirms = new List<Task>();

            foreach (var (@event, properties) in events) {
                var rabbitMQProperties = properties as RabbitMQIntegrationEventProperties;

                if (rabbitMQProperties == null) {
                    throw new ArgumentException($"The type of the properties must be {nameof(RabbitMQIntegrationEventProperties)}");
                }

                var pendingEvent = new PendingEvent(@event.GetType(), @event, rabbitMQProperties);
                await _pendingEvents.Writer.WriteAsync(pendingEvent);

                confirms.Add(pendingEvent.WaitAsync());
            }

            await Task.WhenAll(confirms);
        }

        ValueTask<IPendingEvent> IPendingEvents.PollPendingEvent (CancellationToken cancellationToken) {
            return _pendingEvents.Reader.ReadAsync(cancellationToken);
        }

    }
}
