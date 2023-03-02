namespace EventBus.RabbitMQ {
    public class RabbitMQIncomingIntegrationEventProperties : IIncomingIntegrationEventProperties {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public bool Redelivered { get; set; }
        public bool Persistent { get; set; }
    }
}
