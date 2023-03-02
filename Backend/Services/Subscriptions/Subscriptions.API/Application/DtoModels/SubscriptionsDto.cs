namespace Subscriptions.API.Application.DtoModels {
    public class SubscriptionsDto {
        public int TotalCount { get; set; }
        public List<SubscriptionDto> Subscriptions { get; set; }
    }
}
