using EventBus;

namespace VideoManager.API.Application.IntegrationEvents.Library {
    public class VideoRegisteredToLibraryIntegrationEvent : IntegrationEvent<VideoRegisteredToLibraryIntegrationEventTopic> {
        public Guid VideoId { get; set; }

        public VideoRegisteredToLibraryIntegrationEvent (Guid videoId) {
            VideoId = videoId;
        }
    }

    public class VideoRegisteredToLibraryIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Library." + properties.TopicName;
        }
    }
}
