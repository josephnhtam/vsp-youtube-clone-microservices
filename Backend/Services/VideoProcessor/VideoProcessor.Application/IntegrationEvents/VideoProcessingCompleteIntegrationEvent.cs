using EventBus;
using VideoProcessor.Application.DtoModels;

namespace VideoProcessor.Application.IntegrationEvents {
    public class VideoProcessingCompleteIntegrationEvent : IntegrationEvent<VideoProcessingCompleteIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public List<VideoThumbnailDto> Thumbnails { get; set; }
        public VideoPreviewThumbnailDto? PreviewThumbnail { get; set; }
        public List<ProcessedVideoDto> Videos { get; set; }

        public VideoProcessingCompleteIntegrationEvent () { }

        public VideoProcessingCompleteIntegrationEvent (Guid videoId, List<VideoThumbnailDto> thumbnails, VideoPreviewThumbnailDto? previewThumbnail, List<ProcessedVideoDto> videos) {
            VideoId = videoId;
            Thumbnails = thumbnails;
            Videos = videos;
            PreviewThumbnail = previewThumbnail;
        }
    }

    public class VideoProcessingCompleteIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoProcessor." + properties.TopicName;
        }
    }
}
