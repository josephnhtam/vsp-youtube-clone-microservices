namespace EventBus.RabbitMQ {
    public class RabbitMQQosConfiguration {

        public uint PrefetchSize { get; set; } = 0;
        public ushort PrefetchCount { get; set; } = 32;

    }
}
