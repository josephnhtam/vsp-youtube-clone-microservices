using Domain;
using VideoManager.Domain.DomainEvents;
using VideoManager.Domain.Rules.Videos;

namespace VideoManager.Domain.Models {
    public class Video : DomainEntity, IAggregateRoot {

        private readonly List<ProcessedVideo> _videos;
        private readonly List<VideoThumbnail> _thumbnails;

        public Guid Id { get; private set; }
        public string CreatorId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Tags { get; private set; }
        public Guid? ThumbnailId { get; private set; }
        public VideoStatus Status { get; private set; }
        public VideoVisibility Visibility { get; private set; }
        public bool AllowedToPublish { get; private set; }
        public bool IsPublished => PublishDate != null;

        public VideoProcessingStatus ProcessingStatus { get; private set; }
        public Guid OriginalVideoFileId { get; private set; }
        public string? OriginalVideoFileName { get; private set; }
        public string? OriginalVideoUrl { get; private set; }
        public IReadOnlyList<ProcessedVideo> Videos => _videos.AsReadOnly();

        public VideoThumbnailStatus ThumbnailStatus { get; private set; }
        public IReadOnlyList<VideoThumbnail> Thumbnails => _thumbnails.AsReadOnly();
        public VideoPreviewThumbnail? PreviewThumbnail { get; private set; }

        public VideoMetrics Metrics { get; private set; }
        public DateTimeOffset CreateDate { get; private set; }
        public DateTimeOffset? PublishDate { get; private set; }
        public DateTimeOffset? UnpublishDate { get; private set; }

        public long Version { get; private set; }
        public long InfoVersion { get; private set; }
        public long PublishStatusVersion { get; private set; }

        public VideoThumbnail? Thumbnail {
            get {
                VideoThumbnail? result = null;
                if (ThumbnailId != null) {
                    result = Thumbnails.Where(x => x.ImageFileId == ThumbnailId).FirstOrDefault();
                }
                if (result == null) {
                    result = Thumbnails.FirstOrDefault();
                }
                return result;
            }
        }

        private Video () {
            _videos = new List<ProcessedVideo>();
            _thumbnails = new List<VideoThumbnail>();
        }

        private Video (Guid id, Guid videoFileId, string creatorId, string title, string description) : this() {
            CheckRules(new TitleLengthRule(title),
                       new DescriptionLengthRule(description));

            Id = id;
            CreatorId = creatorId;
            Title = title;
            Description = description;
            ThumbnailId = null;
            Status = VideoStatus.Created;
            Visibility = VideoVisibility.Private;
            Tags = string.Empty;
            AllowedToPublish = true;

            ProcessingStatus = VideoProcessingStatus.WaitingForUserUpload;
            OriginalVideoFileId = videoFileId;
            OriginalVideoFileName = null;
            OriginalVideoUrl = null;

            ThumbnailStatus = VideoThumbnailStatus.Waiting;

            CreateDate = DateTimeOffset.UtcNow;

            Metrics = VideoMetrics.Create();
        }

        public static Video Create (string creatorId, string title, string description) {
            Guid videoId = Guid.NewGuid();
            Guid videoFileId = Guid.NewGuid();
            return new Video(videoId, videoFileId, creatorId, title, description);
        }

        public void SetInfo (string title, string description, string tags) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (title == Title && description == Description && Tags == tags) return;

            CheckRules(new TitleLengthRule(title),
                       new DescriptionLengthRule(description),
                       new TagsLengthRule(tags));

            Title = title;
            Description = description;
            Tags = tags;
            InfoVersion++;

            AddUniqueDomainEvent(new VideoInfoUpdatedDomainEvent(this));
        }

        public void SetThumbnailId (Guid? thumbnailId) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (thumbnailId == ThumbnailId) return;

            if (thumbnailId != null && !Thumbnails.Any(x => x.ImageFileId == thumbnailId)) {
                throw new Exception("Invalid thumbnail id");
            }

            ThumbnailId = thumbnailId;
            InfoVersion++;

            AddUniqueDomainEvent(new VideoInfoUpdatedDomainEvent(this));
        }

        public void SetPublicVisibility () {
            SetVisibility(VideoVisibility.Public);
        }

        public void SetUnlistedVisibility () {
            SetVisibility(VideoVisibility.Unlisted);
        }

        public void SetPrivateVisibility () {
            SetVisibility(VideoVisibility.Private);
        }

        public void SetVisibility (VideoVisibility visibility) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            CheckRule(new ValidVideoVisibilityRule(visibility));

            Visibility = visibility;
            InfoVersion++;

            AddUniqueDomainEvent(new VideoInfoUpdatedDomainEvent(this));
        }

        public void SetAllowedToPublish (bool allowedToPublish) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            AllowedToPublish = allowedToPublish;
            InfoVersion++;

            AddUniqueDomainEvent(new VideoInfoUpdatedDomainEvent(this));
        }

        public void SetVideoRegistered () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Status >= VideoStatus.Registered) {
                throw new Exception("Video is already registered");
            }

            Status = VideoStatus.Registered;

            AddDomainEvent(new VideoRegisteredDomainEvent(this));
        }

        public void SetVideoRegistrationFailed () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Status == VideoStatus.RegistrationFailed) {
                throw new Exception("Video registartion is already failed");
            }

            if (Status == VideoStatus.Registered) {
                throw new Exception("Video is already registered");
            }

            Status = VideoStatus.RegistrationFailed;
        }

        public void SetVideoUploadedStatus (string originalVideoFileName, string originalVideoUrl) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (ProcessingStatus != VideoProcessingStatus.WaitingForUserUpload) {
                throw new Exception("Video is already uploaded");
            }

            OriginalVideoFileName = originalVideoFileName;
            OriginalVideoUrl = originalVideoUrl;
            ProcessingStatus = VideoProcessingStatus.VideoUploaded;

            AddDomainEvent(new VideoUploadedDomainEvent(this));
        }

        public void SetVideoProcessingFailedStatus () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (ProcessingStatus == VideoProcessingStatus.VideoProcessingFailed) {
                throw new Exception("Video processing is already failed");
            }

            ProcessingStatus = VideoProcessingStatus.VideoProcessingFailed;

            AddDomainEvent(new VideoProcessingFailedDomainEvent(this));
        }

        public void SetVideoBeingProcssedStatus () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (ProcessingStatus >= VideoProcessingStatus.VideoBeingProcssed) {
                throw new Exception("Video is already being processed or processed");
            }

            if (ProcessingStatus != VideoProcessingStatus.VideoUploaded) {
                throw new Exception("Video is not uploaded yet");
            }

            ProcessingStatus = VideoProcessingStatus.VideoBeingProcssed;

            AddDomainEvent(new VideoBeingProcessedDomainEvent(this));
        }

        public void SetVideoProcssedStatus () {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (ProcessingStatus == VideoProcessingStatus.VideoProcessed) {
                throw new Exception("Video processing is already complete");
            }

            if (ProcessingStatus == VideoProcessingStatus.VideoProcessingFailed) {
                throw new Exception("Video processing is already failed");
            }

            if (ProcessingStatus == VideoProcessingStatus.WaitingForUserUpload) {
                throw new Exception("Video is not uploaded yet");
            }

            ProcessingStatus = VideoProcessingStatus.VideoProcessed;

            AddDomainEvent(new VideoProcessedDomainEvent(this));
        }

        public void AddProcessedVideo (ProcessedVideo video) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (_videos.Any(x => x.VideoFileId == video.VideoFileId)) {
                throw new Exception("This video is already added");
            }

            _videos.Add(video);

            AddDomainEvent(new ProcessedVideoAddedDomainEvent(this, video));
        }

        public void SetVideos (List<ProcessedVideo> videos) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            _videos.RemoveAll(x => !videos.Any(y => y.VideoFileId == x.VideoFileId));

            foreach (var video in videos) {
                if (!_videos.Any(x => x.VideoFileId == video.VideoFileId)) {
                    _videos.Add(video);
                }
            }
        }

        public void SetThumbnails (List<VideoThumbnail> thumbnails, VideoPreviewThumbnail? previewThumbnail) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            ThumbnailStatus = VideoThumbnailStatus.Processed;

            bool updated = false;

            if (!_thumbnails.SequenceEqual(thumbnails)) {
                _thumbnails.Clear();
                _thumbnails.AddRange(thumbnails);
                updated = true;
            }

            if (PreviewThumbnail != previewThumbnail) {
                PreviewThumbnail = previewThumbnail;
                updated = true;
            }

            if (updated) {
                AddDomainEvent(new VideoThumbnailsUpdatedDomainEvent(this));
            }
        }

        public void SetPublishStatus (bool isPublished, DateTimeOffset date, long version) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (PublishStatusVersion < version) {
                if (isPublished) {
                    PublishDate = date;
                    UnpublishDate = null;
                } else {
                    PublishDate = null;
                    UnpublishDate = date;
                }
                PublishStatusVersion = version;
            }
        }

        public void UpdateVotesMetrics (long likesCount, long dislikesCount, DateTimeOffset updateDate) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Metrics.VotesCountUpdateDate >= updateDate) return;

            Metrics.LikesCount = likesCount;
            Metrics.DislikesCount = dislikesCount;
            Metrics.VotesCountUpdateDate = updateDate;
        }

        public void UpdateViewsMetrics (long viewsCount, DateTimeOffset updateDate) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Metrics.ViewsCountUpdateDate >= updateDate) return;

            Metrics.ViewsCount = viewsCount;
            Metrics.ViewsCountUpdateDate = updateDate;
        }

        public void UpdateCommentsMetrics (long commentsCount, DateTimeOffset updateDate) {
            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            if (Metrics.CommentsCountUpdateDate >= updateDate) return;

            Metrics.CommentsCount = commentsCount;
            Metrics.CommentsCountUpdateDate = updateDate;
        }

        public void SetUnregistered () {
            if (ProcessingStatus > VideoProcessingStatus.WaitingForUserUpload &&
                ProcessingStatus < VideoProcessingStatus.VideoProcessed) {
                throw new Exception("Not ready to unregister");
            }

            if (Status == VideoStatus.Unregistered) {
                throw new Exception("Already unregistered");
            }

            Status = VideoStatus.Unregistered;
            AddDomainEvent(new VideoUnregisteredDomainEvent(this));
        }

        public void IncrementVersion () {
            Version++;
        }

    }
}
