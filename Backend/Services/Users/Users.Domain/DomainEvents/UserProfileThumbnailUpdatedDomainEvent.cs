using Domain.Events;
using Users.Domain.Models;

namespace Users.Domain.DomainEvents {
    public class UserProfileThumbnailUpdatedDomainEvent : IDomainEvent {
        public UserProfile UserProfile { get; set; }
        public ImageFile? OldThumbnail { get; set; }

        public UserProfileThumbnailUpdatedDomainEvent (UserProfile userProfile, ImageFile? oldThumbnail) {
            UserProfile = userProfile;
            OldThumbnail = oldThumbnail;
        }
    }
}
