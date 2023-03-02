using Library.Domain.Models;
using Library.Domain.Specifications;

namespace Library.Domain.Contracts {
    public interface IPlaylistRefRepository {
        Task AddPlaylistRefAsync (PlaylistRef playlistRef, CancellationToken cancellationToken = default);
        Task RemovePlaylistRefAsync (string userId, Guid playlistId, CancellationToken cancellationToken = default);
        Task<bool> HasPlaylistRefAsync (string userId, Guid playlistId, CancellationToken cancellationToken = default);
        Task<PlaylistRef?> GetPlaylistRefAsync (string userId, Guid playlistId, CancellationToken cancellationToken = default);
        Task<List<PlaylistRef>> GetPlaylistRefsAsync (string userId, Pagination? pagination, CancellationToken cancellationToken = default);
        void Track (PlaylistRef playlistRef);
    }
}
