using Domain.Events;

namespace Library.Domain.DomainEvents.Videos {
    public class VideoUnregisteredDomainEvent : IDomainEvent {

        public Guid VideoId { get; set; }

        public VideoUnregisteredDomainEvent (Guid videoId) {
            VideoId = videoId;
        }

    }
}
