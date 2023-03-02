using Community.Domain.DomainEvents;
using Domain;

namespace Community.Domain.Models {
    public class VideoForum : DomainEntity, IAggregateRoot {

        private List<VideoComment> _videoComments;
        private List<VideoCommentVote> _videoCommentVotes;

        public Guid VideoId { get; private set; }
        public UserProfile CreatorProfile { get; private set; }
        public string CreatorId { get; private set; }
        public VideoForumStatus Status { get; private set; }
        public bool AllowedToComment { get; private set; }
        public int VideoCommentsCount { get; private set; }
        public int RootVideoCommentsCount { get; private set; }
        public IReadOnlyList<VideoComment> VideoComments => _videoComments.AsReadOnly();
        public IReadOnlyList<VideoCommentVote> VideoCommentVotes => _videoCommentVotes.AsReadOnly();

        private VideoForum () {
            _videoComments = new List<VideoComment>();
            _videoCommentVotes = new List<VideoCommentVote>();
        }

        private VideoForum (Guid videoId, UserProfile creatorProfile, bool allowedToComment) : this() {
            VideoId = videoId;
            CreatorProfile = creatorProfile;
            CreatorId = creatorProfile.Id;
            AllowedToComment = allowedToComment;
            Status = VideoForumStatus.Registered;

            AddDomainEvent(new VideoForumCreatedDomainEvent(VideoId));
        }

        public static VideoForum Create (Guid videoId, UserProfile creatorProfile, bool allowedToComment) {
            return new VideoForum(videoId, creatorProfile, allowedToComment);
        }

        public VideoComment AddComment (string userId, string comment) {
            if (Status == VideoForumStatus.Unregistered) {
                throw new Exception("Video forum is already unregistered");
            }

            var commentCreated = VideoComment.Create(VideoId, userId, comment);
            _videoComments.Add(commentCreated);
            VideoCommentsCount++;
            RootVideoCommentsCount++;
            AddDomainEvent(new VideoForumCommentAddedDomainEvent(this));
            return commentCreated;
        }

        internal void AddReply (VideoComment reply) {
            if (Status == VideoForumStatus.Unregistered) {
                throw new Exception("Video forum is already unregistered");
            }

            _videoComments.Add(reply);
            VideoCommentsCount++;
            AddDomainEvent(new VideoForumCommentAddedDomainEvent(this));
        }

        public void SetUnregistered () {
            if (Status == VideoForumStatus.Unregistered) {
                throw new Exception("Video forum is already unregistered");
            }

            Status = VideoForumStatus.Unregistered;
            AddDomainEvent(new VideoForumUnregisteredDomainEvent(this));
        }

    }
}
