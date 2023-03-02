using Application.Contracts;

namespace Subscriptions.API.Application.Queries {
    public class GetNotificationMessageCountQuery : IQuery<int> {
        public string UserId { get; set; }

        public GetNotificationMessageCountQuery (string userId) {
            UserId = userId;
        }
    }
}
