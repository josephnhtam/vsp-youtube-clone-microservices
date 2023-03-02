using Application.Contracts;
using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.Commands {
    public class SubscribeToUserCommand : ICommand {
        public string UserId { get; set; }
        public string SubscriptionTargetId { get; set; }
        public NotificationType NotificationType { get; set; }

        public SubscribeToUserCommand (string userId, string subscriptionTargetId, NotificationType notificationType) {
            UserId = userId;
            SubscriptionTargetId = subscriptionTargetId;
            NotificationType = notificationType;
        }
    }
}
