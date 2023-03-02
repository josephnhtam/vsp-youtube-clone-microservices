using Application.Contracts;

namespace Library.API.Application.Commands {
    public class RemovePlaylistRefCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }

        public RemovePlaylistRefCommand (string userId, Guid playlistId) {
            UserId = userId;
            PlaylistId = playlistId;
        }
    }
}
