namespace EventBus.RabbitMQ {
    public class RabbitMQIntegrationEventProperties : IIntegrationEventProperties {

        // Default: Empty
        public string? RoutingKey { get; set; }

        // Default: true
        public bool? Persistent { get; set; }

        // Default: 32. Positive integer between 1 and 255. 
        public byte? Priority { get; set; }

        // Default: null
        public IDictionary<string, object>? Headers { get; set; }

        //Default: false. Set it to true to publish the event directly to the queue (RoutingKey).
        public bool? SkipExchange { get; set; }

    }
}
