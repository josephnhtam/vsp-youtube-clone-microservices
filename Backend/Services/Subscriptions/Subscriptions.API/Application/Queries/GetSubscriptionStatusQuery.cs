using Application.Contracts;
using Subscriptions.API.Application.DtoModels;

namespace Subscriptions.API.Application.Queries {
    public class GetSubscriptionStatusQuery : IQuery<SubscriptionStatusDto?> {
        public string? SubscriptionTargetId { get; set; }
        public string? SubscriptionTargetHandle { get; set; }
        public string? UserId { get; set; }

        public GetSubscriptionStatusQuery (string? subscriptionTargetId, string? subscriptionTargetHandle, string? userId) {
            SubscriptionTargetId = subscriptionTargetId;
            SubscriptionTargetHandle = subscriptionTargetHandle;
            UserId = userId;
        }
    }
}
