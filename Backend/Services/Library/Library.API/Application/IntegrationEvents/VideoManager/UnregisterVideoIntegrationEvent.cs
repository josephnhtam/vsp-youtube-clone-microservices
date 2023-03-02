using EventBus;

namespace Library.API.Application.IntegrationEvents.VideoManager {
    public class UnregisterVideoIntegrationEvent : IntegrationEvent<UnregisterVideoIntegrationEventTopic> {
        public Guid VideoId { get; set; }

        public UnregisterVideoIntegrationEvent () { }

        public UnregisterVideoIntegrationEvent (Guid videoId) {
            VideoId = videoId;
        }
    }

    public class UnregisterVideoIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
