using EventBus;

namespace Community.API.Application.IntegrationEvents {
    public class VideoRegisteredToCommunityIntegrationEvent : IntegrationEvent<VideoRegisteredToCommunityIntegrationEventTopic> {
        public Guid VideoId { get; set; }

        public VideoRegisteredToCommunityIntegrationEvent (Guid videoId) {
            VideoId = videoId;
        }
    }

    public class VideoRegisteredToCommunityIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Community." + properties.TopicName;
        }
    }
}
