using History.Domain.Models;

namespace History.Infrastructure.Contracts {
    public interface ICachedVideoRepository {
        Task<Video?> GetVideoByIdAsync (Guid id, CancellationToken cancellationToken = default);
        Task<List<Video>> GetVideosAsync (IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task CacheVideosAsync (IEnumerable<Video> videos, CancellationToken cancellationToken = default);
        Task RemoveVideoCachesAsync (IEnumerable<Guid> videoIds, CancellationToken cancellationToken = default);
    }
}
