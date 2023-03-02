using Domain.Events;
using Users.Domain.Models;

namespace Users.Domain.DomainEvents {
    public class UserProfileUpdatedDomainEvent : IDomainEvent {
        public UserProfile UserProfile { get; set; }

        public UserProfileUpdatedDomainEvent (UserProfile userProfile) {
            UserProfile = userProfile;
        }
    }
}
