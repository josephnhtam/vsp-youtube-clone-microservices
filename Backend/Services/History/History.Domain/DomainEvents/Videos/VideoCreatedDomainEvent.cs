using Domain.Events;
using History.Domain.Models;

namespace History.Domain.DomainEvents.Videos {
    public class VideoCreatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoCreatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
