using Storage.Domain.Models;

namespace Storage.Domain.Contracts {
    public interface IStorageRepository {
        Task<StoredFile> StoreFileAsync (string? userId, Guid fileId, Guid trackingId, Guid groupId, string category, string? contentType, string fileName, string originalFileName, Stream stream, long? maxFileSize = null, int? bufferSize = null, Func<FileStream, Task>? fileValidator = null, CancellationToken cancellationToken = default);
        Task DeleteFileAsync (StoredFile file);

        Task<bool> HasFileAsync (string category, string? contentType, string fileName, string originalFileName);
        Task DeleteFileAsync (string category, string? contentType, string fileName, string originalFileName);
    }
}
