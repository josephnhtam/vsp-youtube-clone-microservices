using Community.Domain.Models;
using Domain.Events;

namespace Community.Domain.DomainEvents {
    public class VideoCommentVoteCreatedDomainEvent : IDomainEvent {
        public VideoCommentVote VideoCommentVote { get; set; }

        public VideoCommentVoteCreatedDomainEvent (VideoCommentVote videoCommentVote) {
            VideoCommentVote = videoCommentVote;
        }
    }
}
