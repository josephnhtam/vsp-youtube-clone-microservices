using EventBus;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.IntegrationEvents.VideoProcessor {
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
