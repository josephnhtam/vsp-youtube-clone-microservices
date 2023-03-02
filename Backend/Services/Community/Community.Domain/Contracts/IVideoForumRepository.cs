using Community.Domain.Models;

namespace Community.Domain.Contracts {
    public interface IVideoForumRepository {
        Task<VideoForum?> GetVideoForumByIdAsync (Guid videoId);
        Task AddVideoForumAsync (VideoForum videoForum);
        Task RemoveVideoForumAsync (VideoForum videoForum);
    }
}
