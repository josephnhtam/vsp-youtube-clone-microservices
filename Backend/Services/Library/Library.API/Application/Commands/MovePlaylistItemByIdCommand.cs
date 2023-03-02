using Application.Contracts;

namespace Library.API.Application.Commands {
    public class MovePlaylistItemByIdCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public Guid ItemId { get; set; }
        public Guid? PrecedingItemId { get; set; }

        public MovePlaylistItemByIdCommand (string userId, Guid playlistId, Guid itemId, Guid? precedingItemId) {
            UserId = userId;
            PlaylistId = playlistId;
            ItemId = itemId;
            PrecedingItemId = precedingItemId;
        }
    }
}
