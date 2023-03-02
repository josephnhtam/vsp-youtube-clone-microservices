using Microsoft.EntityFrameworkCore;
using VideoProcessor.Domain.Contracts;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Infrastructure.Repositories {
    public class VideoRepository : IVideoRepository {

        private readonly VideoProcessorDbContext _dbContext;

        public VideoRepository (VideoProcessorDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddVideoAsync (Video video, CancellationToken cancellationToken = default) {
            await _dbContext.Videos.AddAsync(video, cancellationToken);
        }

        public Task RemoveVideoAsync (Video video, CancellationToken cancellationToken = default) {
            _dbContext.Remove(video);
            return Task.CompletedTask;
        }

        public async Task<Video?> GetVideoByIdAsync (Guid id, CancellationToken cancellationToken = default) {
            IQueryable<Video> videos;

            if (_dbContext.Database.CurrentTransaction != null) {
                videos = _dbContext.Videos.FromSqlRaw(@"SELECT * FROM ""Videos"" FOR UPDATE");
            } else {
                videos = _dbContext.Videos.AsQueryable();
            }

            return await videos
                .Where(x => x.Id == id)
                .Include(x => x.ProcessingSteps)
                .Include(x => x.Videos)
                .Include(x => x.Thumbnails)
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Video>> GetVideosToProcessAsync (int fetchCount, CancellationToken cancellationToken = default) {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            return await _dbContext.Videos
                .FromSqlRaw(@"SELECT * FROM ""Videos"" FOR UPDATE SKIP LOCKED")
                .Include(x => x.ProcessingSteps)
                .Include(x => x.Videos)
                .Include(x => x.Thumbnails)
                .AsSplitQuery()
                .Where(x => x.Status < VideoProcessingStatus.Processed)
                .Where(x => now >= x.AvailableDate)
                .OrderBy(x => x.AvailableDate)
                .Take(fetchCount)
                .ToListAsync(cancellationToken);
        }

    }
}
