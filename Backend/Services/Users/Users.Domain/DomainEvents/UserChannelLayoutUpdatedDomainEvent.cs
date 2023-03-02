using Domain.Events;
using Users.Domain.Models;

namespace Users.Domain.DomainEvents {
    public class UserChannelLayoutUpdatedDomainEvent : IDomainEvent {
        public UserChannel Channel { get; set; }

        public UserChannelLayoutUpdatedDomainEvent (UserChannel channel) {
            Channel = channel;
        }
    }
}
