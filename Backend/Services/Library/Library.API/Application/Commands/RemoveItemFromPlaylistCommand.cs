using Application.Contracts;

namespace Library.API.Application.Commands {
    public class RemoveItemFromPlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public Guid ItemId { get; set; }

        public RemoveItemFromPlaylistCommand (string userId, Guid playlistId, Guid itemId) {
            UserId = userId;
            PlaylistId = playlistId;
            ItemId = itemId;
        }
    }
}
