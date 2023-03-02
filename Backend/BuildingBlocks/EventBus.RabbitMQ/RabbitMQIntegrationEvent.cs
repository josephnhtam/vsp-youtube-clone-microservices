namespace EventBus.RabbitMQ {
    public class RabbitMQIntegrationEvent {
        public string Type { get; set; }
        public string Data { get; set; }

        public RabbitMQIntegrationEvent (string type, string data) {
            Type = type;
            Data = data;
        }
    }
}
