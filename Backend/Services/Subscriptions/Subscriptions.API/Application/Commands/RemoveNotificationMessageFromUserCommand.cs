using Application.Contracts;

namespace Subscriptions.API.Application.Commands {
    public class RemoveNotificationMessageFromUserCommand : ICommand {
        public string UserId { get; set; }
        public string MessageId { get; set; }

        public RemoveNotificationMessageFromUserCommand (string userId, string messageId) {
            UserId = userId;
            MessageId = messageId;
        }
    }
}
