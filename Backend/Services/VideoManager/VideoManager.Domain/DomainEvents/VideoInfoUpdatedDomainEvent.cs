using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoInfoUpdatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoInfoUpdatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
