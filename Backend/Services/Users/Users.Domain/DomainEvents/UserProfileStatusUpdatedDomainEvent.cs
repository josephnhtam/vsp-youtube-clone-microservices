using Domain.Events;
using Users.Domain.Models;

namespace Users.Domain.DomainEvents {
    public class UserProfileStatusUpdatedDomainEvent : IDomainEvent {
        public UserProfile UserProfile { get; set; }

        public UserProfileStatusUpdatedDomainEvent (UserProfile userProfile) {
            UserProfile = userProfile;
        }
    }
}
