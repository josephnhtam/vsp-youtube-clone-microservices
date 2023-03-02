using EventBus;
using VideoProcessor.Application.DtoModels;

namespace VideoProcessor.Application.IntegrationEvents {
    public class VideoProcessingThumbnailsAddedIntegrationEvent : IntegrationEvent<VideoProcessingThumbnailsAddedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public List<VideoThumbnailDto> Thumbnails { get; set; }
        public VideoPreviewThumbnailDto? PreviewThumbnail { get; set; }

        public VideoProcessingThumbnailsAddedIntegrationEvent () { }

        public VideoProcessingThumbnailsAddedIntegrationEvent (Guid videoId, List<VideoThumbnailDto> thumbnails, VideoPreviewThumbnailDto? previewThumbnail) {
            VideoId = videoId;
            Thumbnails = thumbnails;
            PreviewThumbnail = previewThumbnail;
        }
    }

    public class VideoProcessingThumbnailsAddedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoProcessor." + properties.TopicName;
        }
    }
}
