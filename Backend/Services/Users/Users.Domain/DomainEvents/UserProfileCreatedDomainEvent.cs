using Domain.Events;
using Users.Domain.Models;

namespace Users.Domain.DomainEvents {
    public class UserProfileCreatedDomainEvent : IDomainEvent {
        public UserProfile UserProfile { get; set; }

        public UserProfileCreatedDomainEvent (UserProfile userProfile) {
            UserProfile = userProfile;
        }
    }
}
