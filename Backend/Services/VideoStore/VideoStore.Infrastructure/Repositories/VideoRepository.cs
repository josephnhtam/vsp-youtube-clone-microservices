using Microsoft.EntityFrameworkCore;
using VideoStore.Domain.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.Infrastructure.Repositories {
    public class VideoRepository : IVideoRepository {

        private readonly VideoStoreDbContext _dbContext;

        public VideoRepository (VideoStoreDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddVideoAsync (Video video, CancellationToken cancellationToken = default) {
            await _dbContext.Videos.AddAsync(video, cancellationToken);
        }

        public Task RemoveVideoAsync (Video video, CancellationToken cancellationToken = default) {
            _dbContext.Videos.Remove(video);
            return Task.CompletedTask;
        }

        public async Task<Video?> GetVideoByIdAsync (Guid id, CancellationToken cancellationToken = default) {
            return await _dbContext.Videos
                .Where(x => x.Id == id)
                .Include(x => x.Videos)
                .Include(x => x.CreatorProfile)
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> GetPublicVideosCountAsync (string userId, CancellationToken cancellationToken = default) {
            return await _dbContext.Videos
                 .Where(x =>
                     x.CreatorId == userId &&
                     x.Visibility == VideoVisibility.Public &&
                     x.Status == VideoStatus.Published)
                 .CountAsync(cancellationToken);
        }

    }
}
