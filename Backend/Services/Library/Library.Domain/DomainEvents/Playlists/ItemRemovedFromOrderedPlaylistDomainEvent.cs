using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Playlists {
    public class ItemRemovedFromOrderedPlaylistDomainEvent<TPlaylist> : IDomainEvent
        where TPlaylist : OrderedPlaylistBase<TPlaylist> {
        public Guid PlaylistId { get; set; }
        public Guid ItemId { get; set; }
        public int Position { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public ItemRemovedFromOrderedPlaylistDomainEvent (Guid playlistId, Guid itemId, int position, DateTimeOffset updateDate) {
            PlaylistId = playlistId;
            ItemId = itemId;
            Position = position;
            UpdateDate = updateDate;
        }
    }
}
