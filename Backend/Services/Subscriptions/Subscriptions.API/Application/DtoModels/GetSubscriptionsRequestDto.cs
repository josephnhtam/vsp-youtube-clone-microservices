using Subscriptions.Domain.Specifications;

namespace Subscriptions.API.Application.DtoModels {
    public class GetSubscriptionsRequestDto {
        public SubscriptionTargetSort? Sort { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public bool? IncludeTotalCount { get; set; }
    }
}
