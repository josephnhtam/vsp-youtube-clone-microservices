using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoUnregisteredDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoUnregisteredDomainEvent (Video video) {
            Video = video;
        }

    }
}
