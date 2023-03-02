using Microsoft.AspNetCore.WebUtilities;
using Storage.Domain.Models;

namespace Storage.API.Application.Services {
    public interface IFileStorageHandler {
        Task<StoredFile> StoreFileAsync (
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
            CancellationToken cancellationToken = default);
    }
}
