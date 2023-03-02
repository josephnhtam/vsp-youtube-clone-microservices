using EventBus;

namespace Storage.Shared.IntegrationEvents {
    public class SetFileInUseIntegrationEvent : IntegrationEvent<SetFileInUseIntegrationEventTopic> {
        public List<Guid> FileIds { get; set; }

        public SetFileInUseIntegrationEvent (List<Guid> fileIds) {
            FileIds = fileIds;
        }
    }

    public class SetFileInUseIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Storage.Shared." + properties.TopicName;
        }
    }
}
