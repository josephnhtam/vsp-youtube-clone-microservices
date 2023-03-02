using Domain;

namespace Subscriptions.Domain.Models {
    public class UserProfile : DomainEntity, IAggregateRoot {

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public string? Handle { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public long SubscribersCount { get; private set; }
        public int SubscriptionsCount { get; private set; }

        public long PrimaryVersion { get; private set; }
        public long Version { get; private set; }

        protected UserProfile () {
        }

        private UserProfile (string userId, string displayName, string description, string? handle, string? thumbnailUrl, long primaryVersion) : this() {
            Id = userId;
            DisplayName = displayName;
            Description = description;
            Handle = handle;
            ThumbnailUrl = thumbnailUrl;
            PrimaryVersion = primaryVersion;
        }

        public static UserProfile Create (string userId, string displayName, string description, string? handle, string? thumbnailUrl, long primaryVersion) {
            return new UserProfile(userId, displayName, description, handle, thumbnailUrl, primaryVersion);
        }

        public bool Update (string displayName, string description, string? handle, string? thumbnailUrl, long primaryVersion) {
            if (PrimaryVersion < primaryVersion) {
                DisplayName = displayName;
                Description = description;
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
