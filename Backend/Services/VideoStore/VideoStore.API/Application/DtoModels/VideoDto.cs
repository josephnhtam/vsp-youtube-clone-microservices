using VideoStore.Domain.Models;

namespace VideoStore.API.Application.DtoModels {
    public class VideoDto {
        public Guid Id { get; set; }
        public CreatorProfileDto CreatorProfile { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public VideoVisibility Visibility { get; set; }
        public VideoStatus Status { get; set; }
        public IEnumerable<ProcessedVideoDto> Videos { get; set; }
        public VideoMetricsDto Metrics { get; private set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? PublishDate { get; set; }
    }
}
