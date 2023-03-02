using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Playlists {
    public class ItemRemovedFromUnorderedPlaylistDomainEvent<TPlaylist> : IDomainEvent
        where TPlaylist : UnorderedPlaylistBase<TPlaylist> {
        public Guid PlaylistId { get; set; }
        public Guid ItemId { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public ItemRemovedFromUnorderedPlaylistDomainEvent (Guid playlistId, Guid itemId, DateTimeOffset updateDate) {
            PlaylistId = playlistId;
            ItemId = itemId;
            UpdateDate = updateDate;
        }
    }
}
