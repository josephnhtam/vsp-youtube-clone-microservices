using Domain.Events;
using VideoStore.Domain.Models;

namespace VideoStore.Domain.DomainEvents {
    public class VideoUnpublishedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoUnpublishedDomainEvent (Video video) {
            Video = video;
        }

    }
}
