using EventBus;
using SharedKernel.Utilities;
using System.Text.Json;

namespace Domain.TransactionalEvents.Outbox {
    public class OutboxMessage {

        public const string Category = "Outbox";

        public string EventType { get; set; }
        public string EventData { get; set; }
        public string PropertiesType { get; set; }
        public string PropertiesData { get; set; }

        private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions {
            IncludeFields = true,
            WriteIndented = false
        };

        public OutboxMessage () { }

        public OutboxMessage (string eventType, string eventData, string propertiesType, string propertiesData) {
            EventType = eventType;
            EventData = eventData;
            PropertiesType = propertiesType;
            PropertiesData = propertiesData;
        }

        public OutboxMessage (IntegrationEventBase @event, IIntegrationEventProperties properties) {
            EventType = @event.GetType().FullName!;
            EventData = JsonSerializer.Serialize(@event, @event.GetType());

            PropertiesType = properties.GetType().FullName!;
            PropertiesData = JsonSerializer.Serialize(properties, properties.GetType(), JsonSerializerOptions);
        }

        public bool DeserializeEvent (out IntegrationEventBase @event, out IIntegrationEventProperties properties) {
            if (string.IsNullOrEmpty(EventType) || string.IsNullOrEmpty(EventData) ||
                string.IsNullOrEmpty(PropertiesType) || string.IsNullOrEmpty(PropertiesData)) {
                @event = null!;
                properties = null!;
                return false;
            }

            var eventType = TypeCache.GetType(EventType);
            var propertiesType = TypeCache.GetType(PropertiesType);

            if (eventType == null || propertiesType == null) {
                @event = null!;
                properties = null!;
                return false;
            }

            @event = (JsonSerializer.Deserialize(EventData, eventType, JsonSerializerOptions) as IntegrationEventBase)!;
            properties = (JsonSerializer.Deserialize(PropertiesData, propertiesType, JsonSerializerOptions) as IIntegrationEventProperties)!;

            if (@event == null || propertiesType == null) {
                @event = null!;
                properties = null!;
                return false;
            }

            return true;
        }

        public override string ToString () {
            return $"EventType: {EventType}\nEventData: {EventData}\nPropertiesType: {PropertiesType}\nPropertiesData: {PropertiesData}";
        }

    }
}
