using Domain;
using History.Domain.DomainEvents.Videos;

namespace History.Domain.Models {
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

        public long VideoVersion { get; private set; }

        public bool IsPublic => Status == VideoStatus.Published && Visibility == VideoVisibility.Public;

        public Video () {
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
                Title = title;
                Description = description;
                Tags = tags;
                ThumbnailUrl = thumbnailUrl;
                PreviewThumbnailUrl = previewThumbnailUrl;
                LengthSeconds = lengthSeconds;
                Visibility = visibility;
                Status = status;
                VideoVersion = version;

                AddDomainEvent(new VideoUpdatedDomainEvent(
                    Id,
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
            }
        }

        public void SetViewsMetricsSyncDate (DateTimeOffset? syncDate) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            Metrics.NextSyncDate = syncDate;
            AddDomainEvent(new VideoViewsMetricsSyncDateUpdatedDomainEvent(Id, Metrics.NextSyncDate));
        }

        public void ChangeViewsCount (long viewsCountChange) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            Metrics.ViewsCount = Math.Max(0, Metrics.ViewsCount + viewsCountChange);
            AddDomainEvent(new VideoViewsMetricsChangedDomainEvent(Id, viewsCountChange));
        }

        public void SetUnregistered () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            Status = VideoStatus.Unregistered;
            AddDomainEvent(new VideoUnregisteredDomainEvent(Id));
        }

    }
}
