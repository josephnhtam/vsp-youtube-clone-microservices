using Domain.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;
using Storage.API.Application.Configurations;
using Storage.Domain.Contracts;
using Storage.Domain.Models;

namespace Storage.API.Application.Services {
    public class FileStorageHandler : IFileStorageHandler {

        private readonly IFileRepository _fileRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IFileTrackingRepository _fileUsageTrackingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CleanupConfiguration _config;
        private readonly ILogger<FileStorageHandler> _logger;

        public FileStorageHandler (
            IFileRepository fileRepository,
            IStorageRepository storageRepository,
            IFileTrackingRepository fileUsageTrackingRepository,
            IUnitOfWork unitOfWork,
            IOptions<CleanupConfiguration> config,
            ILogger<FileStorageHandler> logger) {
            _fileRepository = fileRepository;
            _storageRepository = storageRepository;
            _fileUsageTrackingRepository = fileUsageTrackingRepository;
            _unitOfWork = unitOfWork;
            _config = config.Value;
            _logger = logger;
        }

        // Two approaches for file upload.
        // 1: The uploaded file will be initially set in pending-to-remove status,
        // the service that needs it will be responsible to change its status to in-use.
        // 2: The uploaded file will be initially set in in-use status,
        // the service that regrets to upload it will be reponsible to change its status to pending-to-remove status.
        public async Task<StoredFile> StoreFileAsync (
            Guid fileId,
            Guid groupId,
            string category,
            MultipartSection fileUploadSection,
            bool setFileInUseImmediately = false,
            float? fileRemovalDelayHours = null,
            string? creatorId = null,
            List<string>? allowedContentTypes = null,
            long? maxFileSize = null,
            int? bufferSize = null,
            Func<StoredFile, Task>? onStoredFileCreated = null,
            Func<FileStream, Task>? fileValidator = null,
            CancellationToken cancellationToken = default) {
            var fileSection = fileUploadSection.AsFileSection();

            if (fileSection == null || fileSection.FileStream == null) {
                throw new AppException("No file is uploaded", null, StatusCodes.Status400BadRequest);
            }

            string? contentType = fileUploadSection.ContentType;
            if (string.IsNullOrEmpty(contentType) ||
                allowedContentTypes != null && !allowedContentTypes.Contains(contentType)) {
                throw new AppException("Invalid content type", null, StatusCodes.Status400BadRequest);
            }

            if (await _fileRepository.GetFileByIdAsync(fileId) != null) {
                throw new AppException("File already uploaded", null, StatusCodes.Status400BadRequest);
            }

            string fileName = Guid.NewGuid().ToString();
            string originalFileName = fileSection.FileName;
            _logger.LogInformation("File {OriginalFileName} ({FileId}) ({Category}) is being uploaded", originalFileName, fileId, category);

            try {
                var trackingId = await PreStoreFileAsync(
                    groupId: groupId,
                    fileId: fileId,
                    category: category,
                    contentType: contentType,
                    fileName: fileName,
                    originalFileName: originalFileName,
                    fileRemovalDelayHours: fileRemovalDelayHours);

                var storedFile = await _storageRepository.StoreFileAsync(
                    userId: creatorId,
                    fileId: fileId,
                    trackingId: trackingId,
                    groupId: groupId,
                    category: category,
                    contentType: contentType,
                    fileName: fileName,
                    originalFileName: originalFileName,
                    stream: fileSection.FileStream,
                    maxFileSize: maxFileSize,
                    bufferSize: bufferSize,
                    fileValidator: fileValidator,
                    cancellationToken: cancellationToken);

                await PostStoreFileAsync(
                    trackingId: trackingId,
                    storedFile: storedFile,
                    setFileInUseImmediately: setFileInUseImmediately,
                    onStoredFileCreated: onStoredFileCreated);

                _logger.LogInformation("File {FileName} ({FileId}) ({Category}) is uploaded succesfully", fileName, fileId, category);

                return storedFile;
            } catch (BadHttpRequestException ex) {
                _logger.LogError(ex, "File {OriginalFileName} ({FileId}) ({Category}) uploading failed", originalFileName, fileId, category);
                throw new AppException("Upload failed", ex, StatusCodes.Status400BadRequest);
            } catch (Exception ex) {
                _logger.LogError(ex, "File {OriginalFileName} ({FileId}) ({Category}) uploading failed", originalFileName, fileId, category);
                throw;
            }
        }

        // Create file tracking.
        // In case the file storing process is intercepted, the cleanup service is still able to remove it.
        private async Task<Guid> PreStoreFileAsync (Guid groupId, Guid fileId, string category, string? contentType, string fileName, string originalFileName, float? fileRemovalDelayHours) {
            FileTracking? fileTracking = null;

            var fileRemovalDelay = TimeSpan.FromHours(fileRemovalDelayHours ?? _config.DefaultFileRemovalDelayHours);

            await _unitOfWork.ExecuteTransactionAsync(async () => {
                fileTracking = FileTracking.Create(groupId, fileId, category, contentType, fileName, originalFileName, fileRemovalDelay);

                await _fileUsageTrackingRepository.AddFileTrackingAsync(fileTracking);

                await _unitOfWork.CommitAsync();
            });

            return fileTracking!.TrackingId;
        }

        // Add storedFile to the repository.
        // In case the same file ID already exists in the file repository
        // (which may occur when more than one file with the same file ID is uploaded simultaneously),
        // a bad request will be thrown and the redundant file will be removed by the cleanup service
        private async Task PostStoreFileAsync (Guid trackingId, StoredFile storedFile, bool setFileInUseImmediately, Func<StoredFile, Task>? onStoredFileCreated) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var fileTracking = await _fileUsageTrackingRepository.GetFileTrackingByIdAsync(trackingId);
                if (fileTracking == null || fileTracking.Status != FileStatus.PendingToRemove) {
                    throw new TransientException("Inconsistent file tracking state");
                }

                try {
                    if (setFileInUseImmediately) {
                        fileTracking.SetInUse();
                    }

                    await _fileRepository.AddFileAsync(storedFile);

                    if (onStoredFileCreated != null) {
                        await onStoredFileCreated(storedFile);
                    }

                    await _unitOfWork.CommitAsync();
                } catch (Exception ex) when (ex.Identify(ExceptionCategories.UniqueViolation)) {
                    throw new AppException("File already uploaded", null, StatusCodes.Status400BadRequest);
                }
            });
        }

    }
}
