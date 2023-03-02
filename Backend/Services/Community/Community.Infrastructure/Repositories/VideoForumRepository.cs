using Community.Domain.Contracts;
using Community.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Community.Infrastructure.Repositories {
    public class VideoForumRepository : IVideoForumRepository {

        private readonly CommunityDbContext _dbContext;

        public VideoForumRepository (CommunityDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddVideoForumAsync (VideoForum videoForum) {
            await _dbContext.VideoForums.AddAsync(videoForum);
        }

        public async Task<VideoForum?> GetVideoForumByIdAsync (Guid videoId) {
            return await _dbContext.VideoForums
                .FirstOrDefaultAsync(x => x.VideoId == videoId);
        }

        public Task RemoveVideoForumAsync (VideoForum videoForum) {
            _dbContext.VideoForums.Remove(videoForum);
            return Task.CompletedTask;
        }

    }
}
