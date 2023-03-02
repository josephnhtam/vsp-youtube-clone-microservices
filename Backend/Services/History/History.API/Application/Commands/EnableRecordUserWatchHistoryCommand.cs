using Application.Contracts;

namespace History.API.Application.Commands {
    public class EnableRecordUserWatchHistoryCommand : ICommand {
        public string UserId { get; set; }
        public bool Enable { get; set; }

        public EnableRecordUserWatchHistoryCommand (string userId, bool enable) {
            UserId = userId;
            Enable = enable;
        }
    }
}
