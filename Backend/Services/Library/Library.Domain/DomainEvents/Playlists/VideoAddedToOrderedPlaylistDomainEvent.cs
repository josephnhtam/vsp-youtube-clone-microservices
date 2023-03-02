using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Playlists {
    public class VideoAddedToOrderedPlaylistDomainEvent<TPlaylist> : IDomainEvent
        where TPlaylist : OrderedPlaylistBase<TPlaylist> {
        public Guid PlaylistId { get; set; }
        public Guid ItemId { get; set; }
        public Guid VideoId { get; set; }
        public int Position { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public VideoAddedToOrderedPlaylistDomainEvent (Guid playlistId, Guid itemId, Guid videoId, int position, DateTimeOffset updateDate) {
            PlaylistId = playlistId;
            ItemId = itemId;
            VideoId = videoId;
            Position = position;
            UpdateDate = updateDate;
        }
    }
}
