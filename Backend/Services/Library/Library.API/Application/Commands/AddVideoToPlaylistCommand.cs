using Application.Contracts;

namespace Library.API.Application.Commands {
    public class AddVideoToPlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public Guid VideoId { get; set; }

        public AddVideoToPlaylistCommand (string userId, Guid playlistId, Guid videoId) {
            UserId = userId;
            PlaylistId = playlistId;
            VideoId = videoId;
        }
    }
}
