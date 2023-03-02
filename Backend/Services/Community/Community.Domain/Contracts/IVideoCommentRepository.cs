using Community.Domain.Models;
using Community.Domain.Specifications;

namespace Community.Domain.Contracts {
    public interface IVideoCommentRepository {

        Task<VideoComment?> GetVideoCommentByIdAsync (long id, bool includeVideoForum = true, bool includeUserProfile = false);
        Task<List<VideoComment>> GetRootVideoCommentsAsync (Guid videoId, DateTimeOffset? maxDate, int page, int pageSize, VideoCommentSort sort);
        Task<List<VideoComment>> GetUserRootVideoCommentsAsync (Guid videoId, string userId, int maxCount);
        Task<List<VideoComment>> GetVideoCommentRepliesIdAsync (long id, int page, int pageSize);
        Task AddVideoCommentAsync (VideoComment videoComment);
        Task DeleteVideoCommentAsync (VideoComment videoComment);

    }
}
