using EventBus;

namespace VideoProcessor.Application.IntegrationEvents {
    public class VideoProcessingFailedIntegrationEvent : IntegrationEvent<VideoProcessingFailedIntegrationEventTopic> {
        public Guid VideoId { get; set; }

        public VideoProcessingFailedIntegrationEvent () { }

        public VideoProcessingFailedIntegrationEvent (Guid videoId) {
            VideoId = videoId;
        }
    }

    public class VideoProcessingFailedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoProcessor." + properties.TopicName;
        }
    }
}
