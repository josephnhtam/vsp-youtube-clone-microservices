namespace EventBus {
    public interface IEventBus {
        Task PublishEvent (IntegrationEventBase @event, Action<IIntegrationEventProperties>? propertiesConfigurator = null);
        Task PublishEvent (IntegrationEventBase @event, IIntegrationEventProperties properties);
        Task PublishEvents (IEnumerable<(IntegrationEventBase, IIntegrationEventProperties)> events);
        Type GetIntegrationEventPropertiesType ();
    }
}