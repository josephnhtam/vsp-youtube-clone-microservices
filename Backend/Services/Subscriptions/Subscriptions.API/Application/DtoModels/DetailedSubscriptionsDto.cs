namespace Subscriptions.API.Application.DtoModels {
    public class DetailedSubscriptionsDto {
        public int TotalCount { get; set; }
        public List<DetailedSubscriptionDto> Subscriptions { get; set; }
    }
}
