using Application.Contracts;

namespace Library.API.Application.Commands {
    public class MovePlaylistItemCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public Guid ItemId { get; set; }
        public int ToPosition { get; set; }

        public MovePlaylistItemCommand (string userId, Guid playlistId, Guid itemId, int toPosition) {
            UserId = userId;
            PlaylistId = playlistId;
            ItemId = itemId;
            ToPosition = toPosition;
        }
    }
}
