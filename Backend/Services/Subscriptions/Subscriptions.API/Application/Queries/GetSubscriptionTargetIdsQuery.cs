using Application.Contracts;
using Subscriptions.API.Application.DtoModels;

namespace Subscriptions.API.Application.Queries {
    public class GetSubscriptionTargetIdsQuery : IQuery<SubscriptionTargetIdsDto> {
        public string UserId { get; set; }

        public GetSubscriptionTargetIdsQuery (string userId) {
            UserId = userId;
        }
    }
}
