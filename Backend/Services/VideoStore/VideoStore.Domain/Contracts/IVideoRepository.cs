using VideoStore.Domain.Models;

namespace VideoStore.Domain.Contracts {
    public interface IVideoRepository {
        Task AddVideoAsync (Video video, CancellationToken cancellationToken = default);
        Task RemoveVideoAsync (Video video, CancellationToken cancellationToken = default);
        Task<Video?> GetVideoByIdAsync (Guid id, CancellationToken cancellationToken = default);
        Task<int> GetPublicVideosCountAsync (string userId, CancellationToken cancellationToken = default);
    }
}
