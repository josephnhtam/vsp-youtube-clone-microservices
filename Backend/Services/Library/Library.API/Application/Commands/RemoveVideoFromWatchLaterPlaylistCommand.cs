using Application.Contracts;

namespace Library.API.Application.Commands {
    public class RemoveVideoFromWatchLaterPlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid VideoId { get; set; }

        public RemoveVideoFromWatchLaterPlaylistCommand (string userId, Guid videoId) {
            UserId = userId;
            VideoId = videoId;
        }
    }
}
