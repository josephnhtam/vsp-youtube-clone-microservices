using Community.Domain.Models;
using Domain.Events;

namespace Community.Domain.DomainEvents {
    public class VideoForumUnregisteredDomainEvent : IDomainEvent {
        public VideoForum VideoForum { get; set; }

        public VideoForumUnregisteredDomainEvent (VideoForum videoForum) {
            VideoForum = videoForum;
        }
    }
}
