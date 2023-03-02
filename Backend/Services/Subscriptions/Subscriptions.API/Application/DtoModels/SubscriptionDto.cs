namespace Subscriptions.API.Application.DtoModels {
    public class SubscriptionDto {
        public UserProfileDto UserProfile { get; set; }
        public DateTimeOffset SubscriptionDate { get; set; }
    }
}
