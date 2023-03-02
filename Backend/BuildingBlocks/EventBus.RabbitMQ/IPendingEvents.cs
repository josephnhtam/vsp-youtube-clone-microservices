namespace EventBus.RabbitMQ {
    public interface IPendingEvents {
        ValueTask<IPendingEvent> PollPendingEvent (CancellationToken cancellationToken);
    }
}
