using Domain;
using History.Domain.DomainEvents.UserProfiles;

namespace History.Domain.Models {
    public class UserProfile : DomainEntity, IAggregateRoot {

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string? Handle { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public bool RecordWatchHistory { get; private set; }

        public long PrimaryVersion { get; private set; }

        protected UserProfile () { }

        private UserProfile (string userId, string displayName, string? handle, string? thumbnailUrl, long primaryVersion) {
            Id = userId;
            DisplayName = displayName;
            Handle = handle;
            ThumbnailUrl = thumbnailUrl;
            RecordWatchHistory = true;
            PrimaryVersion = primaryVersion;

            AddDomainEvent(new UserProfileCreatedDomainEvent(this));
        }

        public static UserProfile Create (string userId, string displayName, string? handle, string? thumbnailUrl, long primaryVersion) {
            return new UserProfile(userId, displayName, handle, thumbnailUrl, primaryVersion);
        }

        public static UserProfile CreateDummy (string userId) {
            return new UserProfile {
                Id = userId,
            };
        }

        public void Update (string displayName, string? handle, string? thumbnailUrl, long primaryVersion) {
            if (PrimaryVersion < primaryVersion) {
                DisplayName = displayName;
                Handle = handle;
                ThumbnailUrl = thumbnailUrl;
                PrimaryVersion = primaryVersion;

                AddDomainEvent(new UserProfileUpdatedDomainEvent(Id, displayName, handle, thumbnailUrl, primaryVersion));
            }
        }

        public void UpdateRecordWatchHistory (bool enable) {
            RecordWatchHistory = enable;

            AddDomainEvent(new UpdateRecordWatchHistoryDomainEvent(Id, enable));
        }

    }
}
