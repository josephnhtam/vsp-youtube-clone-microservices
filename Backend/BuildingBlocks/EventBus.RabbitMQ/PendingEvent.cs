using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SharedKernel.Utilities;
using System.Text.Json;

namespace EventBus.RabbitMQ {
    public class PendingEvent : PendingEventBase {

        public Type EventType { get; private set; }
        public IntegrationEventBase Event { get; private set; }
        public RabbitMQIntegrationEventProperties Properties { get; private set; }

        private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions {
            IncludeFields = true,
            WriteIndented = false
        };

        public PendingEvent (Type eventType, IntegrationEventBase @event, RabbitMQIntegrationEventProperties properties) {
            EventType = eventType;
            Event = @event;
            Properties = properties;
        }

        public override void Publish (IModel channel, IRabbitMQTopology topology, ILogger logger, Action<ulong>? onObtainSeqNo) {
            string eventTypeName = EventType.GetGenericTypeName();

            var data = JsonSerializer.Serialize((Event as object), EventType, JsonSerializerOptions)!;
            var body = JsonSerializer.SerializeToUtf8Bytes(new RabbitMQIntegrationEvent(eventTypeName, data), JsonSerializerOptions)!;

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = Properties.Persistent ?? true;
            basicProperties.Priority = Properties.Priority ?? 0;

            if (Properties.Headers != null) basicProperties.Headers = Properties.Headers;

            string exchangeName = string.Empty;

            if (!Properties.SkipExchange.HasValue || !Properties.SkipExchange.Value || string.IsNullOrEmpty(Properties.RoutingKey)) {
                var topicDefinition = topology.GetTopicDefinition(EventType)!;

                if (topicDefinition == null) {
                    throw new ArgumentException("The event type is not registered to a topic");
                }

                exchangeName = topicDefinition.ExchangeName;
                logger.LogInformation("Publishing event ({EventTypeName}) ({IntegrationEvent_Id}) to exchange ({ExchangeName})", eventTypeName, Event.Id, topicDefinition.ExchangeName);
            } else {
                logger.LogInformation("Publishing event ({EventTypeName}) ({IntegrationEvent_Id}) to queue ({Queue})", eventTypeName, Event.Id, Properties.RoutingKey);
            }

            var seqNo = channel.NextPublishSeqNo;
            onObtainSeqNo?.Invoke(seqNo);

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: Properties.RoutingKey != null ? Properties.RoutingKey : string.Empty,
                                 basicProperties: basicProperties,
                                 body: body);

        }

    }
}
