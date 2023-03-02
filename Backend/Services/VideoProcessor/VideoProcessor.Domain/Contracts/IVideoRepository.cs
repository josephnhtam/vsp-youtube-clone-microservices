using VideoProcessor.Domain.Models;

namespace VideoProcessor.Domain.Contracts {
    public interface IVideoRepository {
        Task AddVideoAsync (Video video, CancellationToken cancellationToken = default);
        Task RemoveVideoAsync (Video video, CancellationToken cancellationToken = default);
        Task<Video?> GetVideoByIdAsync (Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Video>> GetVideosToProcessAsync (int fetchCount, CancellationToken cancellationToken = default);
    }
}
