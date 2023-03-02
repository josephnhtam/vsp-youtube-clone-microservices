using Domain;

namespace Storage.Domain.Models {
    public class FileTracking : DomainEntity, IAggregateRoot {

        public Guid TrackingId { get; private set; }
        public Guid GroupId { get; private set; }
        public Guid FileId { get; private set; }
        public string Category { get; private set; }
        public string? ContentType { get; private set; }
        public string FileName { get; private set; }
        public string OriginalFileName { get; private set; }

        public FileStatus Status { get; private set; }
        public int RemovalRetryCount { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }
        public DateTimeOffset? RemovalDate { get; private set; }

        public int Version { get; private set; }

        private FileTracking () { }

        private FileTracking (Guid trackingId, Guid groupId, Guid fileId, string category, string? contentType, string fileName, string originalFileName, TimeSpan removalDelay) {
            TrackingId = trackingId;
            GroupId = groupId;
            FileId = fileId;
            Category = category;
            ContentType = contentType;
            FileName = fileName;
            OriginalFileName = originalFileName;
            CreateDate = DateTimeOffset.UtcNow;

            SetPendingToRemove(removalDelay);
        }

        public static FileTracking Create (Guid groupId, Guid fileId, string category, string? contentType, string fileName, string originalFileName, TimeSpan removalDelay) {
            return new FileTracking(Guid.NewGuid(), groupId, fileId, category, contentType, fileName, originalFileName, removalDelay);
        }

        public void SetPendingToRemove (TimeSpan removalDelay) {
            if (Status == FileStatus.Removed) {
                throw new Exception("Already removed");
            }

            Status = FileStatus.PendingToRemove;
            RemovalDate = DateTimeOffset.UtcNow + removalDelay;

            Version++;
        }

        public void SetInUse () {
            if (Status >= FileStatus.Removed) {
                throw new Exception("Already removed");
            }

            Status = FileStatus.InUse;
            RemovalDate = null;

            Version++;
        }

        public void SetRemoved () {
            Status = FileStatus.Removed;
            RemovalDate = null;

            Version++;
        }

        public void SetFailedToRemove () {
            Status = FileStatus.FailedToRemove;
            RemovalDate = null;

            Version++;
        }

        public void RetryToRemove (TimeSpan removalDelay) {
            RemovalRetryCount++;
            SetPendingToRemove(removalDelay);

            Version++;
        }

    }

    public enum FileStatus {
        PendingToRemove,
        InUse,
        Removed,
        FailedToRemove
    }
}
