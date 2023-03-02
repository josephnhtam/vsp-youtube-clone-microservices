namespace EventBus {
    public class IntegrationEventTopic {

        public virtual void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) { }

    }

    public interface IIntegrationEventTopicProperties {
        /// <summary>
        /// Default: Type name of the Topic without namespace
        /// </summary>
        string TopicName { get; set; }
    }
}
