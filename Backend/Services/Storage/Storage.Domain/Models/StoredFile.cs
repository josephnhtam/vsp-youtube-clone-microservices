using Domain;

namespace Storage.Domain.Models {
    public class StoredFile : DomainEntity, IAggregateRoot {

        private readonly List<FileProperty> _properties;

        public Guid FileId { get; private set; }
        public Guid TrackingId { get; private set; }
        public Guid GroupId { get; private set; }
        public string Category { get; private set; }
        public string? ContentType { get; private set; }
        public string FileName { get; private set; }
        public string OriginalFileName { get; private set; }
        public long FileSize { get; private set; }
        public string Url { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }
        public string? UserId { get; private set; }

        public IReadOnlyList<FileProperty> Properties => _properties.AsReadOnly();

        private StoredFile () { }

        private StoredFile (Guid fileId, Guid trackingId, Guid groupId, string? userId, string category, string? contentType, string fileName, string originalFileName, long fileSize, string url, List<FileProperty> claims, DateTimeOffset createDate) {
            FileId = fileId;
            TrackingId = trackingId;
            GroupId = groupId;
            UserId = userId;
            OriginalFileName = originalFileName;
            FileName = fileName;
            ContentType = contentType;
            Category = category;
            FileSize = fileSize;
            Url = url;
            CreateDate = createDate;
            _properties = claims;
        }

        public static StoredFile Create (Guid fileId, Guid trackingId, Guid groupId, string? userId, string category, string? contentType, string fileName, string originalFileName, long fileSize, string url, List<FileProperty> claims, DateTimeOffset createDate) {
            return new StoredFile(fileId, trackingId, groupId, userId, category, contentType, fileName, originalFileName, fileSize, url, claims, createDate);
        }

    }
}
