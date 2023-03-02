using Application.Contracts;

namespace Library.API.Application.Commands {
    public class MoveWatchLaterPlaylistItemByIdCommand : ICommand {
        public string UserId { get; set; }
        public Guid ItemId { get; set; }
        public Guid? PrecedingItemId { get; set; }

        public MoveWatchLaterPlaylistItemByIdCommand (string userId, Guid itemId, Guid? precedingItemId) {
            UserId = userId;
            ItemId = itemId;
            PrecedingItemId = precedingItemId;
        }
    }
}
