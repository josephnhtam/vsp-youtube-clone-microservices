using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoProcessingFailedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoProcessingFailedDomainEvent (Video video) {
            Video = video;
        }

    }
}
