using Domain.Events;

namespace History.Domain.DomainEvents.Videos {
    public class VideoUnregisteredDomainEvent : IDomainEvent {

        public Guid VideoId { get; set; }

        public VideoUnregisteredDomainEvent (Guid videoId) {
            VideoId = videoId;
        }

    }
}
