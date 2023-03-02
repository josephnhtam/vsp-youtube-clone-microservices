using Domain.Events;

namespace Library.Domain.DomainEvents.UserProfiles {
    public class UserProfileUpdatedDomainEvent : IDomainEvent {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long PrimaryVersion { get; set; }

        public UserProfileUpdatedDomainEvent (string id, string displayName, string? handle, string? thumbnailUrl, long primaryVersion) {
            Id = id;
            DisplayName = displayName;
            Handle = handle;
            ThumbnailUrl = thumbnailUrl;
            PrimaryVersion = primaryVersion;
        }
    }
}
