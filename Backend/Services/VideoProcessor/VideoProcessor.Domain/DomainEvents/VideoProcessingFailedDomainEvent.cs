using Domain.Events;

namespace VideoProcessor.Domain.DomainEvents {
    public class VideoProcessingFailedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }

        public VideoProcessingFailedDomainEvent (Guid videoId) {
            VideoId = videoId;
        }
    }
}
