using Storage.Domain.Models;

namespace Storage.Domain.Contracts {
    public interface IFileRepository {
        Task AddFileAsync (StoredFile file);
        Task RemoveFileAsync (StoredFile file);
        Task<StoredFile?> GetFileByIdAsync (Guid fileId);
        Task<StoredFile?> GetFileByTrackingIdAsync (Guid trackingId);
        Task<IEnumerable<StoredFile>> GetFilesAsync (IEnumerable<Guid> fileIds);
    }
}