using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoBeingProcessedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoBeingProcessedDomainEvent (Video video) {
            Video = video;
        }

    }
}
