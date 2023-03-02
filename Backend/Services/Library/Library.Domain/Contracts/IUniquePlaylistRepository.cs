using Library.Domain.Models;
using Library.Domain.Specifications;

namespace Library.Domain.Contracts {
    public interface IUniquePlaylistRepository<T, TItem> : IPlaylistRepository<T, TItem>
        where T : PlaylistBase<TItem> where TItem : PlaylistItem {
        Task<T?> GetPlaylist (string userId, Pagination? itemsPagination, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylist (string userId, int itemsSkip, int itemsLimit, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylist (string userId, bool includeItems = false, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylistIncludingVideo (string userId, Guid videoId, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<T?> GetPlaylistIncludingItem (string userId, Guid itemId, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<bool> IsInPlaylist (string userId, Guid videoId, CancellationToken cancellationToken = default);
        Task<int> GetPlaylistItemPosition (string userId, Guid videoId, CancellationToken cancellationToken = default);
    }
}
