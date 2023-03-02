using Library.Domain.Models;

namespace Library.Infrastructure.Contracts {
    public interface ICachedVideoRepository {
        Task<Video?> GetVideoByIdAsync (Guid id, CancellationToken cancellationToken = default);
        Task<List<Video>> GetVideosAsync (IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<List<Video>> GetAvailableVideosAsync (IEnumerable<Guid> ids, string? userId, CancellationToken cancellationToken = default);
        Task CacheVideosAsync (IEnumerable<Video> videos, CancellationToken cancellationToken = default);
        Task RemoveVideoCachesAsync (IEnumerable<Guid> videoIds, CancellationToken cancellationToken = default);
    }
}
