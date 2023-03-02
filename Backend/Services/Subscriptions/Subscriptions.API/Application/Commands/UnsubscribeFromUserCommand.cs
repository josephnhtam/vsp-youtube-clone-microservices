using Application.Contracts;

namespace Subscriptions.API.Application.Commands {
    public class UnsubscribeFromUserCommand : ICommand {
        public string UserId { get; set; }
        public string SubscriptionTargetId { get; set; }

        public UnsubscribeFromUserCommand (string userId, string subscriptionTargetId) {
            UserId = userId;
            SubscriptionTargetId = subscriptionTargetId;
        }
    }
}
