using Application.Contracts;

namespace Subscriptions.API.Application.Commands {
    public class ResetUnreadNotificationMessageCountCommand : ICommand {
        public string UserId { get; set; }

        public ResetUnreadNotificationMessageCountCommand (string userId) {
            UserId = userId;
        }
    }
}
