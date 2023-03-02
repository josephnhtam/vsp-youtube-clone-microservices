using Community.Domain.Models;
using Domain.Events;

namespace Community.Domain.DomainEvents {
    public class VideoForumCommentAddedDomainEvent : IDomainEvent {
        public VideoForum VideoForum { get; set; }

        public VideoForumCommentAddedDomainEvent (VideoForum videoForum) {
            VideoForum = videoForum;
        }
    }
}
