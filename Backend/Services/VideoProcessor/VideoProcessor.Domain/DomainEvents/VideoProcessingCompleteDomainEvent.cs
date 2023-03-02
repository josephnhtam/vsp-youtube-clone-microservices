using Domain.Events;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Domain.DomainEvents {
    public class VideoProcessingCompleteDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }
        public List<VideoThumbnail> Thumbnails { get; set; }
        public VideoPreviewThumbnail? PreviewThumbnail { get; set; }
        public List<ProcessedVideo> Videos { get; set; }

        public VideoProcessingCompleteDomainEvent (Guid videoId, List<VideoThumbnail> thumbnails, VideoPreviewThumbnail? previewThumbnail, List<ProcessedVideo> videos) {
            VideoId = videoId;
            Thumbnails = thumbnails;
            PreviewThumbnail = previewThumbnail;
            Videos = videos;
        }
    }
}
