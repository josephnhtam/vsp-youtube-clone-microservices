using Application.Contracts;

namespace Subscriptions.API.Application.Commands {
    public class MarkNotificationMessageCheckedCommand : ICommand {
        public string UserId { get; set; }
        public string MessageId { get; set; }

        public MarkNotificationMessageCheckedCommand (string userId, string messageId) {
            UserId = userId;
            MessageId = messageId;
        }
    }
}
