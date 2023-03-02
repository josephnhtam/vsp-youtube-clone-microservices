using Domain.Events;
using VideoStore.Domain.Models;

namespace VideoStore.Domain.DomainEvents {
    public class VideoPublishedWithPublicVisibilityDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoPublishedWithPublicVisibilityDomainEvent (Video video) {
            Video = video;
        }

    }
}
