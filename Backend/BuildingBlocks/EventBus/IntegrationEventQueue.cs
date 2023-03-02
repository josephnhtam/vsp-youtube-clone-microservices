namespace EventBus {
    public class IntegrationEventQueue {

        public virtual void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) { }

    }

    public interface IIntegrationEventQueueProperties {
        /// <summary>
        /// Default: Type name of the Queue without namespace
        /// </summary>
        string QueueName { get; set; }
    }
}
