using Domain.Events;
using VideoStore.Domain.Models;

namespace VideoStore.Domain.DomainEvents {
    public class VideoUnregisteredDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoUnregisteredDomainEvent (Video video) {
            Video = video;
        }

    }
}
