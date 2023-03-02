using History.Domain.Models;

namespace History.Domain.Contracts {
    public interface IVideoRepository {
        Task AddVideoAsync (Video video, CancellationToken cancellationToken = default);
        Task RemoveVideoAsync (Guid videoId, CancellationToken cancellationToken = default);
        Task<Video?> GetVideoByIdAsync (Guid id, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<List<Video>> GetVideosAsync (IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<List<Video>> PollVideoForMetricsSyncAsync (int fetchCount, CancellationToken cancellationToken = default);
        void Track (Video video);
    }
}
