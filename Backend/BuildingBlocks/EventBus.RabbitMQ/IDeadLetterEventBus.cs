namespace EventBus.RabbitMQ {
    public interface IDeadLetterEventBus {
        Task PublishDeadLetterEvent (string deadLetterQueue, ReadOnlyMemory<byte> body, Exception failure);
    }
}
