using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.UserProfiles {
    public class UserProfileCreatedDomainEvent : IDomainEvent {

        public UserProfile UserProfile { get; set; }

        public UserProfileCreatedDomainEvent (UserProfile userProfile) {
            UserProfile = userProfile;
        }

    }
}
