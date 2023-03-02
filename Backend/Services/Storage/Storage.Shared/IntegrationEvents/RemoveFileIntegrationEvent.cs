using EventBus;

namespace Storage.Shared.IntegrationEvents {
    public class RemoveFileIntegrationEvent : IntegrationEvent<RemoveFileIntegrationEventTopic> {
        public List<Guid> FileIds { get; set; }
        public TimeSpan? RemovalDelay { get; set; }

        public RemoveFileIntegrationEvent (List<Guid> fileIds, TimeSpan? removalDelay = null) {
            FileIds = fileIds;
            RemovalDelay = removalDelay;
        }
    }

    public class RemoveFileIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Storage.Shared." + properties.TopicName;
        }
    }
}
