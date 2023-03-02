using Microsoft.EntityFrameworkCore;
using Storage.Domain.Contracts;
using Storage.Domain.Models;

namespace Storage.Infrastructure.Repositories {
    public class FileRepository : IFileRepository {

        private readonly StorageDbContext _dbContext;

        public FileRepository (StorageDbContext context) {
            _dbContext = context;
        }

        public async Task AddFileAsync (StoredFile file) {
            await _dbContext.Files.AddAsync(file);
        }

        public Task RemoveFileAsync (StoredFile file) {
            _dbContext.Files.Remove(file);
            return Task.CompletedTask;
        }

        public async Task<List<StoredFile>> GetFileByGroupIdAsync (Guid groupId) {
            return await _dbContext.Files
                .Where(x => x.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<StoredFile?> GetFileByIdAsync (Guid fileId) {
            return await _dbContext.Files.FirstOrDefaultAsync(x => x.FileId == fileId);
        }

        public async Task<IEnumerable<StoredFile>> GetFilesAsync (IEnumerable<Guid> fileIds) {
            return await _dbContext.Files.Where(x => fileIds.Contains(x.FileId)).ToListAsync();
        }

        public async Task<StoredFile?> GetFileByTrackingIdAsync (Guid trackingId) {
            return await _dbContext.Files.FirstOrDefaultAsync(x => x.TrackingId == trackingId);
        }

    }
}
