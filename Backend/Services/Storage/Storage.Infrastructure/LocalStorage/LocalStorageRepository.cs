using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;
using Storage.Domain.Contracts;
using Storage.Domain.Models;
using Storage.Infrastructure.Scanners;

namespace Storage.Infrastructure.LocalStorage {
    public class LocalStorageRepository : IStorageRepository {

        private readonly IServiceProvider _serviceProvider;
        private readonly LocalStorageConfiguration _config;
        private readonly ILogger<LocalStorageRepository> _logger;

        public LocalStorageRepository (IServiceProvider serviceProvider, IOptions<LocalStorageConfiguration> config, ILogger<LocalStorageRepository> logger) {
            _serviceProvider = serviceProvider;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<StoredFile> StoreFileAsync (string? userId, Guid fileId, Guid trackingId, Guid groupId, string category, string? contentType, string fileName, string originalFileName, Stream stream, long? maxFileSize, int? bufferSize, Func<FileStream, Task>? fileCheck = null, CancellationToken cancellationToken = default) {
            GetPaths(category, contentType, fileName, originalFileName,
                out string relativePath, out string dirPath, out string fullFileName, out string filePath);

            CreateDirectoryIfNotExists(dirPath);

            await StoreFileAsync(contentType, originalFileName, stream, maxFileSize, bufferSize, filePath, fileCheck, cancellationToken);

            await AntiVirusScanAsync(trackingId, filePath);

            await ValidateFileAsync(trackingId, filePath, fileCheck);

            return CreateStoredFile(userId, fileId, trackingId, groupId, category, contentType, fileName, originalFileName, stream, relativePath, fullFileName);
        }

        private StoredFile CreateStoredFile (string? userId, Guid fileId, Guid trackingId, Guid groupId, string category, string? contentType, string fileName, string originalFileName, Stream stream, string relativePath, string fullFileName) {
            long fileSize = stream.Length;

            var uri = Path.Combine(_config.RequestPath, relativePath, fullFileName).Replace('\\', '/');

            var properties = new List<FileProperty> {
                new FileProperty("StorageType", "LocalStorage")
            };

            return StoredFile.Create(fileId, trackingId, groupId, userId, category, contentType, fileName, originalFileName, fileSize, uri, properties, DateTimeOffset.UtcNow);
        }

        private static void CreateDirectoryIfNotExists (string dirPath) {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists) {
                dirInfo.Create();
            }
        }

        private async Task StoreFileAsync (string? contentType, string originalFileName, Stream stream, long? maxFileSize, int? bufferSize, string filePath, Func<FileStream, Task>? fileValidator = null, CancellationToken cancellationToken = default) {
            _logger.LogInformation(@"File {OriginalFileName} ({ContentType}) is being copyed to ""{FilePath}""", originalFileName, contentType, filePath);

            try {
                using (var fileStream = File.Create(filePath)) {
                    byte[] buffer = new byte[(bufferSize * 1024) ?? 4096 * 1024];

                    int bytesRead;
                    long totalBytesRead = 0;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0) {
                        totalBytesRead += bytesRead;

                        if (totalBytesRead > (maxFileSize * 1024)) {
                            throw new AppException("File too large", null, StatusCodes.Status400BadRequest);
                        }

                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    }
                }
            } catch (Exception ex) {
                _logger.LogError(ex, @"An error occurred when storing file {OriginalFileName} ({ContentType}) to ""{FilePath}""", originalFileName, contentType, filePath);
                File.Delete(filePath);
                throw;
            }

            _logger.LogInformation(@"File {OriginalFileName} ({ContentType}) is stored to ""{FilePath}"" successfully", originalFileName, contentType, filePath);
        }

        private async Task AntiVirusScanAsync (Guid trackingId, string filePath) {
            var antiVirusScanner = _serviceProvider.GetService<IAntiVirusScanner>();

            if (antiVirusScanner != null) {
                bool result = await antiVirusScanner.ScanAndClean(filePath);

                if (!result) {
                    _logger.LogError("The file ({TrackingId}) cannot pass the anti-virus scanner", trackingId);
                    throw new Exception("Virus detected");
                }
            }
        }

        private async Task ValidateFileAsync (Guid trackingId, string filePath, Func<FileStream, Task>? fileValidator) {
            if (fileValidator == null) return;

            try {
                using (var fileStream = File.OpenRead(filePath)) {
                    await fileValidator.Invoke(fileStream);
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to validate the the file ({TrackingId})", trackingId);
                File.Delete(filePath);
                throw;
            }
        }

        private static string GetFullFileName (string fileName, string originalFileName) {
            return fileName + Path.GetExtension(originalFileName);
        }

        private static string GetRelativePath (string? contentType, string category) {
            return Path.Combine(category,
                string.IsNullOrEmpty(contentType) ? "other" : LocalStorageHelper.GetDirectory(contentType));
        }

        public Task DeleteFileAsync (StoredFile file) {
            return DeleteFileAsync(file.Category, file.ContentType, file.FileName, file.OriginalFileName);
        }

        public Task<bool> HasFileAsync (string category, string? contentType, string fileName, string originalFileName) {
            GetPaths(category, contentType, fileName, originalFileName,
                out string relativePath, out string dirPath, out string fullFileName, out string filePath);

            return Task.FromResult(File.Exists(filePath));
        }

        public Task DeleteFileAsync (string category, string? contentType, string fileName, string originalFileName) {
            GetPaths(category, contentType, fileName, originalFileName,
                out string relativePath, out string dirPath, out string fullFileName, out string filePath);

            if (File.Exists(filePath)) {
                try {
                    File.Delete(filePath);
                    _logger.LogInformation(@"File {FullFileName} stored at ""{FilePath}"" is deleted", fullFileName, filePath);
                } catch (Exception ex) {
                    if (ex is not FileNotFoundException) {
                        throw;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void GetPaths (string category, string? contentType, string fileName, string originalFileName, out string relativePath, out string dirPath, out string fullFileName, out string filePath) {
            var rootPath = LocalStorageHelper.GetRootPath(_config);
            relativePath = GetRelativePath(contentType, category);
            dirPath = Path.Combine(rootPath, relativePath);
            fullFileName = GetFullFileName(fileName, originalFileName);
            filePath = Path.Combine(dirPath, fullFileName);
        }
    }
}
