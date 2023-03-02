namespace EventBus {
    /// <summary>
    /// Cast this interface to different type according to the infrastructure
    /// to provide infrastructure-specific properties
    /// (e.g. RabbitMQIntegrationEventProperties)
    /// </summary>
    public interface IIntegrationEventProperties {
    }
}
