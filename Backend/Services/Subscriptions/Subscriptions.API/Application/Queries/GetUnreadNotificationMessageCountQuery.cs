using Application.Contracts;

namespace Subscriptions.API.Application.Queries {
    public class GetUnreadNotificationMessageCountQuery : IQuery<int> {
        public string UserId { get; set; }

        public GetUnreadNotificationMessageCountQuery (string userId) {
            UserId = userId;
        }
    }
}
