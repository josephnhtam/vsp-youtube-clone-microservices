using Storage.Domain.Models;

namespace Storage.Domain.Contracts {
    public interface IFileTrackingRepository {
        Task AddFileTrackingAsync (FileTracking fileTracking);
        Task RemoveFileTrackingAsync (FileTracking fileTracking);
        Task<FileTracking?> GetFileTrackingByIdAsync (Guid trackingId, CancellationToken cancellationToken = default);
        Task<IEnumerable<FileTracking>> GetFileTrackingsByFileIdsAsync (IEnumerable<Guid> trackingIds, CancellationToken cancellationToken = default);
        Task<IEnumerable<FileTracking>> GetFileTrackingsByGroupIdAsync (Guid groupId, CancellationToken cancellationToken = default);
        Task<IEnumerable<FileTracking>> PollFilesToRemoveAsync (int fetchCount, CancellationToken cancellationToken = default);
    }
}
