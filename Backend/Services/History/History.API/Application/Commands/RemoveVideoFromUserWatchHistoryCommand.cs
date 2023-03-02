using Application.Contracts;

namespace History.API.Application.Commands {
    public class RemoveVideoFromUserWatchHistoryCommand : ICommand {
        public string UserId { get; set; }
        public Guid VideoId { get; set; }

        public RemoveVideoFromUserWatchHistoryCommand (string userId, Guid videoId) {
            UserId = userId;
            VideoId = videoId;
        }
    }
}
