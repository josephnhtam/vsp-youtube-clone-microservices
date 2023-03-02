using Community.Domain.Models;
using Domain.Events;

namespace Community.Domain.DomainEvents {
    public class VideoCommentVoteChangedDomainEvent : IDomainEvent {
        public VideoCommentVote VideoCommentVote { get; set; }
        public VoteType Previous { get; set; }
        public VoteType Current { get; set; }

        public VideoCommentVoteChangedDomainEvent (VideoCommentVote videoCommentVote, VoteType previous, VoteType current) {
            VideoCommentVote = videoCommentVote;
            Previous = previous;
            Current = current;
        }
    }
}
