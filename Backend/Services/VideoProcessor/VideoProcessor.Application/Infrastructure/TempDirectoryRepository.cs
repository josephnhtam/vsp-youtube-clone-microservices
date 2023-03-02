using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using VideoProcessor.Application.Configurations;

namespace VideoProcessor.Application.Infrastructure {
    public class TempDirectoryRepository : ITempDirectoryRepository {

        private readonly TempDirectoryDbContext _dbContext;
        private readonly LocalTempStorageConfiguration _tempStorageConfig;
        private readonly ILogger<TempDirectoryRepository> _logger;

        public TempDirectoryRepository (TempDirectoryDbContext dbContext, IOptions<LocalTempStorageConfiguration> tempStorageConfig, ILogger<TempDirectoryRepository> logger) {
            _dbContext = dbContext;
            _tempStorageConfig = tempStorageConfig.Value;
            _logger = logger;
        }

        public async Task<string> GetTempDirectoryAsync (Guid id) {
            var tmpDir = await _dbContext.TempDirectories.FirstOrDefaultAsync(x => x.Id == id);

            string dirPath;

            if (tmpDir == null) {
                dirPath = Path.Combine(_tempStorageConfig.Path, Dns.GetHostName(), Guid.NewGuid().ToString());

                await _dbContext.TempDirectories.AddAsync(new TempDirectory(id, dirPath));
                await _dbContext.SaveChangesAsync();
            } else {
                dirPath = tmpDir.Path;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists) {
                dirInfo.Create();
            }

            return dirPath;
        }

        public async Task RemoveTempDirectoryAsync (Guid id) {
            var tmpDir = await _dbContext.TempDirectories.FirstOrDefaultAsync(x => x.Id == id);

            if (tmpDir != null) {
                await DoRemoveTempDirectoryAsync(tmpDir);
            }
        }

        public async Task RemoveAllTempDirectoriesAsync () {
            var tmpDirs = await _dbContext.TempDirectories.ToListAsync();

            foreach (var tmpDir in tmpDirs) {
                await DoRemoveTempDirectoryAsync(tmpDir);
            }
        }

        private async Task DoRemoveTempDirectoryAsync (TempDirectory tmpDir) {
            DirectoryInfo dirInfo = new DirectoryInfo(tmpDir.Path);
            if (dirInfo.Exists) {
                try {
                    dirInfo.Delete(true);
                } catch (IOException ex) {
                    _logger.LogError(ex, "Failed to remove temp directory");
                    throw;
                }
            }

            _dbContext.TempDirectories.Remove(tmpDir);
            await _dbContext.SaveChangesAsync();
        }

    }
}
