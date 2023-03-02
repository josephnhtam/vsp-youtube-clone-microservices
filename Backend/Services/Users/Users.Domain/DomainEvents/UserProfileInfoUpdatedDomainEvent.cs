using Domain.Events;
using Users.Domain.Models;

namespace Users.Domain.DomainEvents {
    public class UserProfileInfoUpdatedDomainEvent : IDomainEvent {
        public UserProfile UserProfile { get; set; }

        public UserProfileInfoUpdatedDomainEvent (UserProfile userProfile) {
            UserProfile = userProfile;
        }
    }
}
