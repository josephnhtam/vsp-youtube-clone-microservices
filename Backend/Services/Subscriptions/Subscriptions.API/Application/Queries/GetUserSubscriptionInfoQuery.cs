using Application.Contracts;
using Subscriptions.API.Application.DtoModels;

namespace Subscriptions.API.Application.Queries {
    public class GetUserSubscriptionInfoQuery : IQuery<UserSubscriptionInfoDto> {
        public string UserId { get; set; }

        public GetUserSubscriptionInfoQuery (string userId) {
            UserId = userId;
        }
    }
}
