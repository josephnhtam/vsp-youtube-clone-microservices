using Community.Domain.DomainEvents;
using Community.Domain.Rules.VideoCommentVotes;
using Domain;

namespace Community.Domain.Models {
    public class VideoCommentVote : DomainEntity, IAggregateRoot {

        public string UserId { get; private set; }
        public long VideoCommentId { get; private set; }
        public Guid VideoId { get; private set; }
        public VoteType Type { get; private set; }

        public long Version { get; private set; }

        private VideoCommentVote () { }

        private VideoCommentVote (string userId, long videoCommentId, Guid videoId, VoteType type) {
            CheckRule(new ValidVoteTypeRule(type));

            UserId = userId;
            VideoCommentId = videoCommentId;
            VideoId = videoId;
            Type = type;

            AddDomainEvent(new VideoCommentVoteCreatedDomainEvent(this));
        }

        public static VideoCommentVote Create (string userId, long videoCommentId, Guid videoId, VoteType type) {
            return new VideoCommentVote(userId, videoCommentId, videoId, type);
        }

        public void ChangeVoteType (VoteType type) {
            CheckRule(new ValidVoteTypeRule(type));

            var previous = Type;
            Type = type;
            AddDomainEvent(new VideoCommentVoteChangedDomainEvent(this, previous, Type));
        }

        public void IncrementVersion () {
            Version++;
        }

    }
}
