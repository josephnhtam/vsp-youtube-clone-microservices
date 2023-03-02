using Application.Contracts;

namespace Library.API.Application.Commands {
    public class RemoveItemFromWatchLaterPlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid ItemId { get; set; }

        public RemoveItemFromWatchLaterPlaylistCommand (string userId, Guid itemId) {
            UserId = userId;
            ItemId = itemId;
        }
    }
}
