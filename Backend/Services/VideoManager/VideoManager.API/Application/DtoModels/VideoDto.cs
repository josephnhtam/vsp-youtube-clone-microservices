using VideoManager.Domain.Models;

namespace VideoManager.API.Application.DtoModels {
    public class VideoDto {
        public Guid Id { get; set; }
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public Guid? ThumbnailId { get; set; }
        public VideoStatus Status { get; set; }
        public VideoVisibility Visibility { get; set; }
        public bool AllowedToPublish { get; set; }

        public VideoProcessingStatus ProcessingStatus { get; set; }
        public Guid OriginalVideoFileId { get; set; }
        public string? OriginalVideoFileName { get; set; }
        public string? OriginalVideoUrl { get; set; }
        public IEnumerable<ProcessedVideoDto> Videos { get; set; }

        public VideoThumbnailStatus ThumbnailStatus { get; set; }
        public IEnumerable<VideoThumbnailDto> Thumbnails { get; set; }

        public VideoMetricsDto Metrics { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
