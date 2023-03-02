using Library.Domain.Models;
using Library.Domain.Specifications;

namespace Library.Domain.Contracts {
    public interface IPlaylistRepository<T, TItem> where T : PlaylistBase<TItem> where TItem : PlaylistItem {
        Task AddPlaylistAsync (T playlist, CancellationToken cancellationToken = default);
        Task RemovePlaylistAsync (Guid id, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylist (Guid id, int itemsSkip, int itemsLimit, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylist (Guid id, Pagination? itemsPagination, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylist (Guid id, bool includeItems = false, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylistIncludingVideo (Guid id, Guid videoId, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylistIncludingItem (Guid id, Guid itemId, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<List<T>> GetPlaylists (List<Guid> playlistIds, int maxItemsCount, bool publicOnly, CancellationToken cancellationToken = default);
        Task<List<T>> GetPlaylistsIncludingVideo (string userId, Guid videoId, CancellationToken cancellationToken = default);
        Task<int> GetPlaylistItemPosition (Guid id, Guid videoId, CancellationToken cancellationToken = default);
        void Track (T playlist);
    }
}
