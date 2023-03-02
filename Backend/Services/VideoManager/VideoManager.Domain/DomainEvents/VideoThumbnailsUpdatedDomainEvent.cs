using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoThumbnailsUpdatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoThumbnailsUpdatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
