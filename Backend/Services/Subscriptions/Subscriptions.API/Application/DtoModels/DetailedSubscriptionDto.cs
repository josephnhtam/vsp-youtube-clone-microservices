using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.DtoModels {
    public class DetailedSubscriptionDto {
        public DetailedUserProfileDto UserProfile { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTimeOffset SubscriptionDate { get; set; }
    }
}
