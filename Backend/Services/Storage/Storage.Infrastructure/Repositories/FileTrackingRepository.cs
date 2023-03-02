using Microsoft.EntityFrameworkCore;
using Storage.Domain.Contracts;
using Storage.Domain.Models;

namespace Storage.Infrastructure.Repositories {
    public class FileTrackingRepository : IFileTrackingRepository {

        private readonly StorageDbContext _dbContext;

        public FileTrackingRepository (StorageDbContext context) {
            _dbContext = context;
        }

        public async Task AddFileTrackingAsync (FileTracking fileTracking) {
            await _dbContext.AddAsync(fileTracking);
        }

        public async Task<FileTracking?> GetFileTrackingByIdAsync (Guid fileId, CancellationToken cancellationToken = default) {
            return await _dbContext.FileTrackings
                .Where(x => x.TrackingId == fileId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<FileTracking>> GetFileTrackingsByFileIdsAsync (IEnumerable<Guid> fileIds, CancellationToken cancellationToken = default) {
            fileIds = fileIds.Distinct();

            return await _dbContext.FileTrackings
                .Where(x => fileIds.Contains(x.FileId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FileTracking>> GetFileTrackingsByGroupIdAsync (Guid groupId, CancellationToken cancellationToken = default) {
            return await _dbContext.FileTrackings
                .Where(x => x.GroupId == groupId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FileTracking>> PollFilesToRemoveAsync (int fetchCount, CancellationToken cancellationToken = default) {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            IQueryable<FileTracking> query;

            if (_dbContext.Database.CurrentTransaction != null) {
                query = _dbContext.FileTrackings
                    .FromSqlRaw(@"SELECT * FROM ""FileTrackings"" FOR UPDATE SKIP LOCKED");
            } else {
                query = _dbContext.FileTrackings.AsQueryable();
            }

            return await query
                .Where(x => x.Status == FileStatus.PendingToRemove && x.RemovalDate < now)
                .OrderBy(x => x.RemovalDate)
                .Take(fetchCount)
                .ToListAsync(cancellationToken);
        }

        public Task RemoveFileTrackingAsync (FileTracking fileTracking) {
            _dbContext.Remove(fileTracking);
            return Task.CompletedTask;
        }

    }
}
