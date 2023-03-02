using Application.Contracts;

namespace Library.API.Application.Commands {
    public class RemovePlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }

        public RemovePlaylistCommand (string userId, Guid playlistId) {
            UserId = userId;
            PlaylistId = playlistId;
        }
    }
}
