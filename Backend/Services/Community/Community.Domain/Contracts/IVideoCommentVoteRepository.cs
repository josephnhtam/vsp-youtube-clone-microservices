using Community.Domain.Models;

namespace Community.Domain.Contracts {
    public interface IVideoCommentVoteRepository {

        Task<VideoCommentVote?> GetVideoCommentVoteAsync (string userId, long videoCommentId);
        Task AddVideoCommentVoteAsync (VideoCommentVote videoCommentVote);
        Task<List<UserVideoCommentVote>> GetVotedVideoCommentsAsync (Guid videoId, string userId);

    }
}
