using Domain.Events;

namespace VideoProcessor.Domain.DomainEvents {
    public class VideoProcessingStartedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }

        public VideoProcessingStartedDomainEvent (Guid videoId) {
            VideoId = videoId;
        }
    }
}
