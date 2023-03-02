using Domain.Events;
using VideoStore.Domain.Models;

namespace VideoStore.Domain.DomainEvents {
    public class VideoCreatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoCreatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
