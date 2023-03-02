using Domain;
using VideoStore.Domain.DomainEvents;

namespace VideoStore.Domain.Models {
    public class Video : DomainEntity, IAggregateRoot {

        private readonly List<ProcessedVideo> _videos;

        public Guid Id { get; private set; }
        public string CreatorId { get; private set; }
        public UserProfile CreatorProfile { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Tags { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public string? PreviewThumbnailUrl { get; private set; }
        public VideoVisibility Visibility { get; private set; }
        public bool AllowedToPublish { get; private set; }
        public bool PublishedWithPublicVisibility { get; private set; }
        public VideoStatus Status { get; private set; }
        public IReadOnlyList<ProcessedVideo> Videos => _videos.AsReadOnly();
        public VideoMetrics Metrics { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }
        public DateTimeOffset? StatusUpdateDate { get; private set; }

        public long Version { get; private set; }
        public long InfoVersion { get; private set; }

        private Video () {
            _videos = new List<ProcessedVideo>();
            Metrics = VideoMetrics.Create();
        }

        private Video (Guid id, UserProfile creatorProfile, string title, string description, string tags, VideoVisibility visibility, DateTimeOffset createDate) : this() {
            Id = id;
            CreatorId = creatorProfile.Id;
            CreatorProfile = creatorProfile;
            Title = title;
            Description = description;
            Tags = tags;
            ThumbnailUrl = null;
            PreviewThumbnailUrl = null;
            Visibility = visibility;
            AllowedToPublish = false;
            PublishedWithPublicVisibility = false;
            Status = VideoStatus.Preparing;
            CreateDate = createDate;

            AddDomainEvent(new VideoCreatedDomainEvent(this));
        }

        public static Video CreateVideo (Guid id, UserProfile creatorProfile, string title, string description, string tags, VideoVisibility visibility, DateTimeOffset createDate) {
            return new Video(id, creatorProfile, title, description, tags, visibility, createDate);
        }

        public void SetInfo (string title, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, VideoVisibility visibility, bool allowedToPublish, long infoVersion, bool force = false) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (InfoVersion < infoVersion || (force && InfoVersion == infoVersion)) {
                Title = title;
                Description = description;
                Tags = tags;
                ThumbnailUrl = thumbnailUrl;
                PreviewThumbnailUrl = previewThumbnailUrl;
                Visibility = visibility;
                AllowedToPublish = allowedToPublish;
                InfoVersion = infoVersion;

                CheckForPublishability(DateTimeOffset.UtcNow);
                CheckForPublishWithPublicVisibility();

                if (Status == VideoStatus.Published) {
                    AddUniqueDomainEvent(new VideoUpdatedDomainEvent(this));
                }
            }
        }

        public void SetVideoResources (List<ProcessedVideo>? videos) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (videos != null) {
                _videos.Clear();
                _videos.AddRange(videos);
            } else {
                _videos.Clear();
            }
        }

        public void SetReadyStatus () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Status >= VideoStatus.Ready) {
                throw new Exception("Already ready or published");
            }

            var updateDate = DateTimeOffset.UtcNow;

            Status = VideoStatus.Ready;
            StatusUpdateDate = updateDate;

            CheckForPublishability(updateDate);
            CheckForPublishWithPublicVisibility();

            AddUniqueDomainEvent(new VideoUpdatedDomainEvent(this));
        }

        public void SetUnregisteredStatus () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            Status = VideoStatus.Unregistered;
            StatusUpdateDate = DateTimeOffset.UtcNow;

            AddDomainEvent(new VideoUnregisteredDomainEvent(this));
        }

        private void CheckForPublishWithPublicVisibility () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (!PublishedWithPublicVisibility &&
                Status == VideoStatus.Published &&
                Visibility == VideoVisibility.Public) {
                PublishedWithPublicVisibility = true;
                AddUniqueDomainEvent(new VideoPublishedWithPublicVisibilityDomainEvent(this));
            }
        }

        private void CheckForPublishability (DateTimeOffset date) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Status == VideoStatus.Ready && AllowedToPublish) {
                Status = VideoStatus.Published;
                StatusUpdateDate = date;

                AddUniqueDomainEvent(new VideoPublishedDomainEvent(this));
            }

            if (Status == VideoStatus.Published && !AllowedToPublish) {
                Status = VideoStatus.Ready;
                StatusUpdateDate = date;

                AddUniqueDomainEvent(new VideoUnpublishedDomainEvent(this));
            }
        }

        public void UpdateViewsMetrics (long viewsCount, DateTimeOffset upadateDate) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Metrics.ViewsCountUpdateDate >= upadateDate) return;

            Metrics.ViewsCount = viewsCount;
            Metrics.ViewsCountUpdateDate = upadateDate;
        }

        public void IncrementVersion () {
            Version++;
        }

    }
}
