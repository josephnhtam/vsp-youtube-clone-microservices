using Domain.Events;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Domain.DomainEvents {
    public class VideoProcessingVideoAddedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }
        public ProcessedVideo Video { get; set; }

        public VideoProcessingVideoAddedDomainEvent (Guid videoId, ProcessedVideo video) {
            VideoId = videoId;
            Video = video;
        }
    }
}
