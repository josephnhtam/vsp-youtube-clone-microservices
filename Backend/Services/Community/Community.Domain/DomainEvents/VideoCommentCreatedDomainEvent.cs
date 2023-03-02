using Community.Domain.Models;
using Domain.Events;

namespace Community.Domain.DomainEvents {
    public class VideoCommentCreatedDomainEvent : IDomainEvent {
        public VideoComment VideoComment { get; set; }

        public VideoCommentCreatedDomainEvent (VideoComment videoComment) {
            VideoComment = videoComment;
        }
    }
}
