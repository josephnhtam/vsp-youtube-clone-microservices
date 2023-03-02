using Domain.Events;

namespace Community.Domain.DomainEvents {
    public class VideoForumCreatedDomainEvent : IDomainEvent {
        public Guid VideoId { get; set; }

        public VideoForumCreatedDomainEvent (Guid videoId) {
            VideoId = videoId;
        }
    }
}
