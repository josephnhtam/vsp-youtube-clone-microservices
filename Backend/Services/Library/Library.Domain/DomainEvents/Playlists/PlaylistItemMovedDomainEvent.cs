using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Playlists {
    public class PlaylistItemMovedDomainEvent<TPlaylist> : IDomainEvent
        where TPlaylist : OrderedPlaylistBase<TPlaylist> {
        public Guid PlaylistId { get; set; }
        public int FromPosition { get; set; }
        public int ToPosition { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public PlaylistItemMovedDomainEvent (Guid playlistId, int fromPosition, int toPosition, DateTimeOffset updateDate) {
            PlaylistId = playlistId;
            FromPosition = fromPosition;
            ToPosition = toPosition;
            UpdateDate = updateDate;
        }
    }
}
