using EventBus;

namespace Library.API.Application.IntegrationEvents {
    public class RemoveVideoSearchInfoIntegrationEvent : IntegrationEvent<RemoveVideoSearchInfoIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public long Version { get; set; }

        public RemoveVideoSearchInfoIntegrationEvent () { }

        public RemoveVideoSearchInfoIntegrationEvent (Guid videoId, long version) {
            VideoId = videoId;
            Version = version;
        }
    }

    public class RemoveVideoSearchInfoIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Library." + properties.TopicName;
        }
    }
}
