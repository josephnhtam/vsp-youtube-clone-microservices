using EventBus;

namespace VideoProcessor.Application.IntegrationEvents {
    public class VideoProcessingStartedIntegrationEvent : IntegrationEvent<VideoProcessingStartedIntegrationEventTopic> {
        public Guid VideoId { get; set; }

        public VideoProcessingStartedIntegrationEvent () { }

        public VideoProcessingStartedIntegrationEvent (Guid videoId) {
            VideoId = videoId;
        }
    }

    public class VideoProcessingStartedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoProcessor." + properties.TopicName;
        }
    }
}
