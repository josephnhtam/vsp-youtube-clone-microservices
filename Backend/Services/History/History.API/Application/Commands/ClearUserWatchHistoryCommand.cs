using Application.Contracts;

namespace History.API.Application.Commands {
    public class ClearUserWatchHistoryCommand : ICommand {
        public string UserId { get; set; }

        public ClearUserWatchHistoryCommand (string userId) {
            UserId = userId;
        }
    }
}
