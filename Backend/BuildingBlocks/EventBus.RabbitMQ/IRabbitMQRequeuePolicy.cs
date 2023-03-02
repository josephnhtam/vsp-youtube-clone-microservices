namespace EventBus.RabbitMQ {
    /// <summary>
    /// Used to detemine if the event should be requeued
    /// when an exception thrown from the handler
    /// </summary>
    public interface IRabbitMQRequeuePolicy {
        bool ShouldRequeue (Exception exception);
    }
}
