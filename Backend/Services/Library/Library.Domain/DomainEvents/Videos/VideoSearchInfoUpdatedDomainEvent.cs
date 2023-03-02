using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Videos {
    public class VideoSearchInfoUpdatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoSearchInfoUpdatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
