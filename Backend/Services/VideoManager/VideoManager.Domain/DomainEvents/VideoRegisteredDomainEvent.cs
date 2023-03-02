using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoRegisteredDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoRegisteredDomainEvent (Video video) {
            Video = video;
        }

    }
}
