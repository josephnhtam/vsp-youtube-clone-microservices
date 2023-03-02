using Domain.Events;
using Users.Domain.Models;

namespace Users.Domain.DomainEvents {
    public class UserChannelBannerUpdatedDomainEvent : IDomainEvent {
        public UserChannel UserChannel { get; set; }
        public ImageFile? OldBanner { get; set; }

        public UserChannelBannerUpdatedDomainEvent (UserChannel userChannel, ImageFile? oldBanner) {
            UserChannel = userChannel;
            OldBanner = oldBanner;
        }
    }
}
