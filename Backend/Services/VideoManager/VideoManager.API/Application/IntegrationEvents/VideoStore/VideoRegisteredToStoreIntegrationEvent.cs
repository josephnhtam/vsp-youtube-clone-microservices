using EventBus;

namespace VideoManager.API.Application.IntegrationEvents.VideoStore {
    public class VideoRegisteredToStoreIntegrationEvent : IntegrationEvent<VideoRegisteredToStoreIntegrationEventTopic> {
        public Guid VideoId { get; set; }

        public VideoRegisteredToStoreIntegrationEvent (Guid videoId) {
            VideoId = videoId;
        }
    }

    public class VideoRegisteredToStoreIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoStore." + properties.TopicName;
        }
    }
}
