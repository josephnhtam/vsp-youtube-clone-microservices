namespace EventBus.RabbitMQ {
    public class RabbitMQIncomingIntegrationEventContext : IIncomingIntegrationEventContext {
        public bool? RequeueWhenNack { get; set; }
    }
}
