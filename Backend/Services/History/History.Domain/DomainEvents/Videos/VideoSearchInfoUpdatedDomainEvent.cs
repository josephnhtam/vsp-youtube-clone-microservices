using Domain.Events;
using History.Domain.Models;

namespace History.Domain.DomainEvents.Videos {
    public class VideoSearchInfoUpdatedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoSearchInfoUpdatedDomainEvent (Video video) {
            Video = video;
        }

    }
}
