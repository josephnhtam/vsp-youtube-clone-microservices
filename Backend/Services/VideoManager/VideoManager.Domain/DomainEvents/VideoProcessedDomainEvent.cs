using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoProcessedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoProcessedDomainEvent (Video video) {
            Video = video;
        }

    }
}
