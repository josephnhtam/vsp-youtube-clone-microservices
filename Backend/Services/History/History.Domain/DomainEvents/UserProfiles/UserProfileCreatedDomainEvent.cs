using Domain.Events;
using History.Domain.Models;

namespace History.Domain.DomainEvents.UserProfiles {
    public class UserProfileCreatedDomainEvent : IDomainEvent {

        public UserProfile UserProfile { get; set; }

        public UserProfileCreatedDomainEvent (UserProfile userProfile) {
            UserProfile = userProfile;
        }

    }
}
