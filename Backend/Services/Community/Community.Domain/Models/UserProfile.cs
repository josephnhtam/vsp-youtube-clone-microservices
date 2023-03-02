using Domain;

namespace Community.Domain.Models {
    public class UserProfile : DomainEntity, IAggregateRoot {

        private List<VideoForum> _videoForums;
        private List<VideoCommentVote> _videoCommentVotes;

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string? Handle { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public IReadOnlyList<VideoForum> VideoForums => _videoForums.AsReadOnly();
        public IReadOnlyList<VideoCommentVote> VideoCommentVotes => _videoCommentVotes.AsReadOnly();

        public long PrimaryVersion { get; private set; }
        public long Version { get; set; }

        private UserProfile () {
            _videoForums = new List<VideoForum>();
            _videoCommentVotes = new List<VideoCommentVote>();
        }

        private UserProfile (string userId, string displayName, string? handle, string? thumbnailUrl, long primaryVersion) : this() {
            Id = userId;
            DisplayName = displayName;
            Handle = handle;
            ThumbnailUrl = thumbnailUrl;
            PrimaryVersion = primaryVersion;
        }

        public static UserProfile Create (string userId, string displayName, string? handle, string? thumbnailUrl, long primaryVersion) {
            return new UserProfile(userId, displayName, handle, thumbnailUrl, primaryVersion);
        }

        public VideoForum AddVideoForum (Guid videoId, bool allowedToComment) {
            var videoForum = VideoForum.Create(videoId, this, allowedToComment);
            _videoForums.Add(videoForum);
            return videoForum;
        }

        public bool Update (string displayName, string? handle, string? thumbnailUrl, long primaryVersion) {
            if (PrimaryVersion < primaryVersion) {
                DisplayName = displayName;
                Handle = handle;
                ThumbnailUrl = thumbnailUrl;
                PrimaryVersion = primaryVersion;
                return true;
            }
            return false;
        }

        public void IncrementVersion () {
            Version++;
        }

    }
}
