using Microsoft.EntityFrameworkCore;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;
using VideoManager.Domain.Specifications;

namespace VideoManager.Infrastructure.Repositories {
    public class VideoRepository : IVideoRepository {

        private readonly VideoManagerDbContext _dbContext;

        public VideoRepository (VideoManagerDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddVideoAsync (Video video) {
            await _dbContext.AddAsync(video);
        }

        public async Task<Video?> GetVideoByIdAsync (Guid id) {
            return await _dbContext.Videos
                .Where(x => x.Id == id && x.Status != VideoStatus.Unregistered)
                .Include(x => x.Videos)
                .Include(x => x.Thumbnails)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public async Task<List<Video>> GetVideosByUserIdAsync (string userId, int page, int pageSize, VideoSort sort) {
            var videos = _dbContext.Videos
                .Where(x => x.CreatorId == userId && x.Status != VideoStatus.Unregistered);

            switch (sort) {
                default:
                case VideoSort.DateDesc:
                    videos = videos.OrderByDescending(x => x.CreateDate);
                    break;
                case VideoSort.DateAsc:
                    videos = videos.OrderBy(x => x.CreateDate);
                    break;
                case VideoSort.ViewsDesc:
                    videos = videos.OrderByDescending(x => x.Metrics.ViewsCount).ThenByDescending(x => x.CreateDate);
                    break;
                case VideoSort.ViewsAsc:
                    videos = videos.OrderBy(x => x.Metrics.ViewsCount).ThenByDescending(x => x.CreateDate);
                    break;
            }

            return await videos
                .Skip(Math.Max(0, (page - 1) * pageSize))
                .Take(pageSize)
                .Include(x => x.Videos)
                .Include(x => x.Thumbnails)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetVideosCountByUserIdAsync (string userId) {
            return await _dbContext.Videos
                .CountAsync(x => x.CreatorId == userId && x.Status != VideoStatus.Unregistered);
        }

        public Task RemoveVideoAsync (Video video) {
            _dbContext.Videos.Remove(video);
            return Task.CompletedTask;
        }

        public void UpdateVideo (Video video) {
            _dbContext.Update(video);
        }
    }
}
