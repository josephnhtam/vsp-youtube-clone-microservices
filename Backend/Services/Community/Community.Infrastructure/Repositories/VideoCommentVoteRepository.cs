using Community.Domain.Contracts;
using Community.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Community.Infrastructure.Repositories {
    public class VideoCommentVoteRepository : IVideoCommentVoteRepository {

        private readonly CommunityDbContext _dbContext;

        public VideoCommentVoteRepository (CommunityDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<VideoCommentVote?> GetVideoCommentVoteAsync (string userId, long videoCommentId) {
            return await _dbContext.VideoCommentVotes
                .Where(x => x.UserId == userId && x.VideoCommentId == videoCommentId)
                .FirstOrDefaultAsync();
        }

        public async Task AddVideoCommentVoteAsync (VideoCommentVote videoCommentVote) {
            await _dbContext.VideoCommentVotes.AddAsync(videoCommentVote);
        }

        public async Task<List<UserVideoCommentVote>> GetVotedVideoCommentsAsync (Guid videoId, string userId) {
            return await _dbContext.VideoCommentVotes
                .Where(x => x.UserId == userId && x.VideoId == videoId)
                .Select(x => new UserVideoCommentVote(x.VideoCommentId, x.Type))
                .ToListAsync();
        }

    }
}
