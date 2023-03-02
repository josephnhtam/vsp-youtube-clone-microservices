using Library.Domain.Models;
using Library.Domain.Specifications;

namespace Library.Infrastructure.Contracts {
    public interface IPlaylistQueryHelper {
        Task<GetPlaylistsResult> GetPlaylistsAsync (string userId, bool publicOnly, int maxItemsCount, Pagination? playlistPagination, CancellationToken cancellationToken = default);
        Task<GetPlaylistsResult> GetPlaylistsIncludingRefAsync (string userId, int maxItemsCount, Pagination? playlistPagination, CancellationToken cancellationToken = default);
    }

    public class GetPlaylistsResult {
        public int TotalCount { get; set; }
        public List<Playlist> Playlists { get; set; }
    }
}
