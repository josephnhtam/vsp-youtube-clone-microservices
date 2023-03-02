using EventBus;
using VideoProcessor.Application.DtoModels;

namespace VideoProcessor.Application.IntegrationEvents {
    public class VideoProcessingVideoAddedIntegrationEvent : IntegrationEvent<VideoProcessingVideoAddedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public ProcessedVideoDto Video { get; set; }

        public VideoProcessingVideoAddedIntegrationEvent () { }

        public VideoProcessingVideoAddedIntegrationEvent (Guid videoId, ProcessedVideoDto video) {
            VideoId = videoId;
            Video = video;
        }
    }

    public class VideoProcessingVideoAddedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoProcessor." + properties.TopicName;
        }
    }
}
