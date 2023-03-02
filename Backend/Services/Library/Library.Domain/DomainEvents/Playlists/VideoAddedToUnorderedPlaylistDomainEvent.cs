using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Playlists {
    public class VideoAddedToUnorderedPlaylistDomainEvent<TPlaylist> : IDomainEvent
        where TPlaylist : UnorderedPlaylistBase<TPlaylist> {
        public Guid PlaylistId { get; set; }
        public Guid ItemId { get; set; }
        public Guid VideoId { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public VideoAddedToUnorderedPlaylistDomainEvent (Guid playlistId, Guid itemId, Guid videoId, DateTimeOffset updateDate) {
            PlaylistId = playlistId;
            ItemId = itemId;
            VideoId = videoId;
            UpdateDate = updateDate;
        }
    }
}
