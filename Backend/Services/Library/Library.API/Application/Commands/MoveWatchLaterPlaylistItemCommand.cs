using Application.Contracts;

namespace Library.API.Application.Commands {
    public class MoveWatchLaterPlaylistItemCommand : ICommand {
        public string UserId { get; set; }
        public Guid ItemId { get; set; }
        public int ToPosition { get; set; }

        public MoveWatchLaterPlaylistItemCommand (string userId, Guid itemId, int toPosition) {
            UserId = userId;
            ItemId = itemId;
            ToPosition = toPosition;
        }
    }
}
