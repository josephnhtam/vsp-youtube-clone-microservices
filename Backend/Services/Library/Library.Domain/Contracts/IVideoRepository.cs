using Library.Domain.Models;
using Library.Domain.Specifications;

namespace Library.Domain.Contracts {
    public interface IVideoRepository {
        Task AddVideoAsync (Video video, CancellationToken cancellationToken = default);
        Task RemoveVideoAsync (Guid videoId, CancellationToken cancellationToken = default);
        Task<Video?> GetVideoByIdAsync (Guid id, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<List<Video>> GetVideosAsync (IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<List<Video>> GetVideosAsync (string userId, bool publicOnly, Pagination pagination, VideoSort sort, CancellationToken cancellationToken = default);
        Task<List<Video>> GetAvailableVideosAsync (IEnumerable<Guid> ids, string userId, CancellationToken cancellationToken = default);
        Task<List<Video>> PollVideoForMetricsSyncAsync (int fetchCount, CancellationToken cancellationToken = default);
        Task<int> GetVideosCount (string userId, bool publicOnly, CancellationToken cancellationToken = default);
        void Track (Video video);
    }
}
