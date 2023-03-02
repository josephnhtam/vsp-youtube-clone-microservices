using Application.Contracts;

namespace Library.API.Application.Commands {
    public class AddVideoToWatchLaterPlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid VideoId { get; set; }

        public AddVideoToWatchLaterPlaylistCommand (string userId, Guid videoId) {
            UserId = userId;
            VideoId = videoId;
        }
    }
}
