using EventBus;

namespace VideoManager.API.Application.IntegrationEvents.Community {
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
