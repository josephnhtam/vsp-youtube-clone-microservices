using Domain.Events;
using VideoStore.Domain.Models;

namespace VideoStore.Domain.DomainEvents {
    public class VideoUpdatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoUpdatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
