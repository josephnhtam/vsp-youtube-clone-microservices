using VideoManager.Domain.Models;
using VideoManager.Domain.Specifications;

namespace VideoManager.Domain.Contracts {
    public interface IVideoRepository {
        Task AddVideoAsync (Video video);
        Task RemoveVideoAsync (Video video);
        Task<Video?> GetVideoByIdAsync (Guid id);
        Task<List<Video>> GetVideosByUserIdAsync (string userId, int page, int pageSize, VideoSort sort);
        Task<int> GetVideosCountByUserIdAsync (string userId);
        void UpdateVideo (Video video);
    }
}
