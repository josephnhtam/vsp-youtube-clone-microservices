using Application.Contracts;

namespace Library.API.Application.Commands {
    public class CreatePlaylistRefCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }

        public CreatePlaylistRefCommand (string userId, Guid playlistId) {
            UserId = userId;
            PlaylistId = playlistId;
        }
    }
}
