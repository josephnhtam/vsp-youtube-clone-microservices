using Domain.Events;
using VideoStore.Domain.Models;

namespace VideoStore.Domain.DomainEvents {
    public class VideoPublishedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoPublishedDomainEvent (Video video) {
            Video = video;
        }

    }
}
