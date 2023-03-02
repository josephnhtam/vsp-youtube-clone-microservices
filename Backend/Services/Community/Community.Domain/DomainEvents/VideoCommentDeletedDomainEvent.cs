using Community.Domain.Models;
using Domain.Events;

namespace Community.Domain.DomainEvents {
    public class VideoCommentDeletedDomainEvent : IDomainEvent {
        public VideoComment VideoComment { get; set; }

        public VideoCommentDeletedDomainEvent (VideoComment videoComment) {
            VideoComment = videoComment;
        }
    }
}
