using Domain;
using Library.Domain.DomainEvents.Videos;

namespace Library.Domain.Models {
    public class Video : DomainEntity, IAggregateRoot {

        public Guid Id { get; private set; }
        public string CreatorId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Tags { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public string? PreviewThumbnailUrl { get; private set; }
        public int? LengthSeconds { get; private set; }
        public VideoVisibility Visibility { get; private set; }
        public VideoStatus Status { get; private set; }
        public VideoMetrics Metrics { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }
        public DateTimeOffset? StatusUpdateDate { get; private set; }

        public long VideoVersion { get; private set; }

        public bool IsPublic => Status == VideoStatus.Published && Visibility == VideoVisibility.Public;

        private Video () {
            Metrics = VideoMetrics.Create();
        }

        private Video (Guid id, string creatorId, string title, string description, string tags, VideoVisibility visibility, DateTimeOffset createDate) : this() {
            Id = id;
            CreatorId = creatorId;
            Title = title;
            Description = description;
            Tags = tags;
            Visibility = visibility;
            Status = VideoStatus.Preparing;
            CreateDate = createDate;

            AddDomainEvent(new VideoCreatedDomainEvent(this));
        }

        public static Video Create (Guid id, string creatorId, string title, string description, string tags, VideoVisibility visibility, DateTimeOffset createDate) {
            return new Video(id, creatorId, title, description, tags, visibility, createDate);
        }

        public void Update (string title, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, int? lengthSeconds, VideoVisibility visibility, VideoStatus status, DateTimeOffset? statusUpdateDate, long version) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (VideoVersion < version) {
                bool wasPublic = IsPublic;

                Title = title;
                Description = description;
                Tags = tags;
                ThumbnailUrl = thumbnailUrl;
                PreviewThumbnailUrl = previewThumbnailUrl;
                LengthSeconds = lengthSeconds;
                Visibility = visibility;
                Status = status;
                StatusUpdateDate = statusUpdateDate;
                VideoVersion = version;

                AddDomainEvent(new VideoUpdatedDomainEvent(
                    Id,
                    CreatorId,
                    title,
                    description,
                    tags,
                    thumbnailUrl,
                    previewThumbnailUrl,
                    lengthSeconds,
                    visibility,
                    status,
                    statusUpdateDate,
                    version));

                bool isPublic = IsPublic;

                if (isPublic || wasPublic != isPublic) {
                    AddDomainEvent(new VideoSearchInfoUpdatedDomainEvent(this));
                }
            }
        }

        public void UpdateViewsMetrics (long viewsCount, DateTimeOffset updateDate) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Metrics.ViewsCountUpdateDate >= updateDate) return;

            Metrics.ViewsCount = viewsCount;
            Metrics.ViewsCountUpdateDate = updateDate;

            AddDomainEvent(new VideoViewsMetricsUpdatedDomainEvent(Id, viewsCount, updateDate));
        }

        public void ChangeVoteMetrics (long likesCountChange, long dislikesCountChange, TimeSpan syncDelay) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            Metrics.LikesCount = Math.Max(0, Metrics.LikesCount + likesCountChange);
            Metrics.DislikesCount = Math.Max(0, Metrics.DislikesCount + dislikesCountChange);

            if (Metrics.NextSyncDate == null) {
                Metrics.NextSyncDate = DateTimeOffset.UtcNow + syncDelay;
                AddDomainEvent(new VideoVoteMetricsChangedDomainEvent(Id, likesCountChange, dislikesCountChange, Metrics.NextSyncDate.Value));
            } else {
                AddDomainEvent(new VideoVoteMetricsChangedDomainEvent(Id, likesCountChange, dislikesCountChange, null));
            }
        }

        public void SetUnregistered () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            Status = VideoStatus.Unregistered;

            AddDomainEvent(new VideoSearchInfoUpdatedDomainEvent(this));
            AddDomainEvent(new VideoUnregisteredDomainEvent(Id));
        }

    }
}
