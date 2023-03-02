using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Videos {
    public class VideoCreatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoCreatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
