using Domain;
using VideoProcessor.Domain.DomainEvents;

namespace VideoProcessor.Domain.Models {
    public interface IReadOnlyVideo {
        public Guid Id { get; }
        public string CreatorId { get; }
        public string OriginalFileName { get; }
        public string VideoFileUrl { get; }
        public VideoInfo? VideoInfo { get; }
        public VideoProcessingStatus Status { get; }
        public DateTimeOffset AvailableDate { get; }
        public DateTimeOffset? ProcessedDate { get; }
        public int RetryCount { get; }
        public VideoPreviewThumbnail? PreviewThumbnail { get; }
        public IReadOnlyList<VideoProcessingStep> ProcessingSteps { get; }
        public IReadOnlyList<VideoThumbnail> Thumbnails { get; }
        public IReadOnlyList<ProcessedVideo> Videos { get; }
        public int LockVersion { get; }
    }

    public class Video : DomainEntity, IAggregateRoot, IReadOnlyVideo {

        private readonly List<VideoProcessingStep> _processingSteps;
        private readonly List<VideoThumbnail> _thumbnails;
        private readonly List<ProcessedVideo> _videos;

        public Guid Id { get; private set; }
        public string CreatorId { get; private set; }
        public string OriginalFileName { get; private set; }
        public string VideoFileUrl { get; private set; }
        public VideoInfo? VideoInfo { get; private set; }
        public VideoProcessingStatus Status { get; private set; }
        public DateTimeOffset AvailableDate { get; private set; }
        public DateTimeOffset? ProcessedDate { get; private set; }
        public int RetryCount { get; private set; }
        public VideoPreviewThumbnail? PreviewThumbnail { get; private set; }
        public IReadOnlyList<VideoProcessingStep> ProcessingSteps => _processingSteps.AsReadOnly();
        public IReadOnlyList<VideoThumbnail> Thumbnails => _thumbnails.AsReadOnly();
        public IReadOnlyList<ProcessedVideo> Videos => _videos.AsReadOnly();

        public int LockVersion { get; private set; }

        private Video () {
            _processingSteps = new List<VideoProcessingStep>();
            _thumbnails = new List<VideoThumbnail>();
            _videos = new List<ProcessedVideo>();
        }

        private Video (Guid id, string creatorId, string originalFileName, string videoFileUrl, List<VideoProcessingStep> processingSteps) : this() {
            Id = id;
            CreatorId = creatorId;
            OriginalFileName = originalFileName;
            VideoFileUrl = videoFileUrl;

            Status = VideoProcessingStatus.Pending;
            AvailableDate = DateTimeOffset.UtcNow;
            RetryCount = 0;

            _processingSteps = processingSteps;
        }

        public static Video CreateDummy (Guid id) {
            return new Video {
                Id = id
            };
        }

        public static Video Create (Guid id, string creatorId, string originalFileName, string videoFileUrl, List<VideoProcessingStep> processingSteps) {
            return new Video(id, creatorId, originalFileName, videoFileUrl, processingSteps);
        }

        public void SetProcessingStarted () {
            Status = VideoProcessingStatus.ProcessingThumbnails;

            AddDomainEvent(new VideoProcessingStartedDomainEvent(Id));
        }

        public void AddThumbnails (IEnumerable<VideoThumbnail> thumbnails, VideoPreviewThumbnail? previewThumbnail) {
            if (Status == VideoProcessingStatus.ProcessingThumbnails) {
                _thumbnails.AddRange(thumbnails);
                PreviewThumbnail = previewThumbnail;
                Status = VideoProcessingStatus.ProcessingVideos;

                AddDomainEvent(new VideoProcessingThumbnailsAddedDomainEvent(Id, Thumbnails.ToList(), PreviewThumbnail));
            }
        }

        public void AddVideo (ProcessedVideo processedVideo) {
            if (Status == VideoProcessingStatus.ProcessingVideos) {
                _videos.Add(processedVideo);

                AddDomainEvent(new VideoProcessingVideoAddedDomainEvent(Id, processedVideo));
            }
        }

        public void SetProcessed () {
            if (Status == VideoProcessingStatus.ProcessingVideos) {
                Status = VideoProcessingStatus.Processed;
                AddDomainEvent(new VideoProcessingCompleteDomainEvent(Id, Thumbnails.ToList(), PreviewThumbnail, Videos.ToList()));
            }
        }

        public void SetFailed () {
            if (Status < VideoProcessingStatus.Processed) {
                Status = VideoProcessingStatus.Failed;

                AddDomainEvent(new VideoProcessingFailedDomainEvent(Id));
            }
        }

        public void SetVideoInfo (VideoInfo videoInfo) {
            VideoInfo = videoInfo;
        }

        public void PostponeAvailableDate (TimeSpan duration) {
            AvailableDate = DateTimeOffset.UtcNow + duration;
        }

        public void RetryLater (TimeSpan delay) {
            RetryCount++;
            AvailableDate = DateTimeOffset.UtcNow + delay;
        }

        public void IncrementLockVersion () {
            LockVersion++;
        }

    }
}
