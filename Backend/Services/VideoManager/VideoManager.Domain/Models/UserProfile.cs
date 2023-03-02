using Domain;

namespace VideoManager.Domain.Models {
    public class UserProfile : DomainEntity, IAggregateRoot {

        private readonly List<Video> _videos;

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Handle { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public IReadOnlyList<Video> Videos => _videos.AsReadOnly();

        public long PrimaryVersion { get; private set; }
        public long Version { get; private set; }

        private UserProfile () {
            _videos = new List<Video>();
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

        public Video AddVideo (string title, string description) {
            var videoForum = Video.Create(Id, title, description);
            _videos.Add(videoForum);
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
