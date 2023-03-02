using Community.Domain.DomainEvents;
using Community.Domain.Rules.VideoComments;
using Domain;

namespace Community.Domain.Models {
    public class VideoComment : DomainEntity, IAggregateRoot {

        private List<VideoComment> _replies;
        private List<VideoCommentVote> _videoCommentVotes;

        public long Id { get; private set; }
        public Guid VideoId { get; private set; }
        public VideoForum VideoForum { get; private set; }
        public long? ParentCommentId { get; private set; }
        public VideoComment? ParentComment { get; private set; }

        public string UserId { get; private set; }
        public UserProfile UserProfile { get; private set; }
        public string Comment { get; private set; }
        public int LikesCount { get; private set; }
        public int DislikesCount { get; private set; }
        public int RepliesCount { get; private set; }
        public IReadOnlyList<VideoComment> Replies => _replies.AsReadOnly();
        public IReadOnlyList<VideoCommentVote> VideoCommentVotes => _videoCommentVotes.AsReadOnly();
        public DateTimeOffset CreateDate { get; private set; }
        public DateTimeOffset? EditDate { get; private set; }

        private VideoComment () {
            _replies = new List<VideoComment>();
            _videoCommentVotes = new List<VideoCommentVote>();
        }

        private VideoComment (Guid videoId, string userId, string comment, long? parentCommentId = null) : this() {
            CheckRule(new CommentLengthRule(comment));

            VideoId = videoId;
            UserId = userId;
            Comment = comment;
            ParentCommentId = parentCommentId;
            CreateDate = DateTimeOffset.UtcNow;
            EditDate = null;

            AddDomainEvent(new VideoCommentCreatedDomainEvent(this));
        }

        internal static VideoComment Create (Guid videoId, string userId, string comment, long? parentCommentId = null) {
            return new VideoComment(videoId, userId, comment, parentCommentId);
        }

        public VideoComment Reply (string userId, string comment) {
            CheckRule(new CommentLengthRule(comment));

            var commentCreated = VideoComment.Create(VideoId, userId, comment, Id);

            if (VideoForum != null) {
                VideoForum.AddReply(commentCreated);
            }

            _replies.Add(commentCreated);
            RepliesCount++;
            return commentCreated;
        }

        public void Edit (string comment) {
            CheckRule(new CommentLengthRule(comment));

            Comment = comment;
            EditDate = DateTimeOffset.UtcNow;
        }

        public void Delete () {
            AddDomainEvent(new VideoCommentDeletedDomainEvent(this));
        }

    }
}
