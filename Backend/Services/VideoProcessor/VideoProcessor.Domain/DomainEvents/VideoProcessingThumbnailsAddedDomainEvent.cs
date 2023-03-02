using Domain.Events;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Domain.DomainEvents {
    public class VideoProcessingThumbnailsAddedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }
        public List<VideoThumbnail> Thumbnails { get; set; }
        public VideoPreviewThumbnail? PreviewThumbnail { get; set; }

        public VideoProcessingThumbnailsAddedDomainEvent (Guid videoId, List<VideoThumbnail> thumbnails, VideoPreviewThumbnail? previewThumbnail) {
            VideoId = videoId;
            Thumbnails = thumbnails;
            PreviewThumbnail = previewThumbnail;
        }
    }
}
