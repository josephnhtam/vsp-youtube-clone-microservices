using Library.Domain.Models;

namespace Library.API.Application.DtoModels {
    public abstract class VideoDtoBase {
        public Guid Id { get; set; }
    }

    public class HiddenVideoDto : VideoDtoBase {
        public VideoVisibility? Visibility { get; set; }

        public HiddenVideoDto (Guid id, VideoVisibility? visibility) {
            Id = id;
            Visibility = visibility;
        }
    }

    public class VideoDto : VideoDtoBase {
        public CreatorProfileDto CreatorProfile { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public int LengthSeconds { get; set; }
        public VideoVisibility Visibility { get; set; }
        public VideoStatus Status { get; set; }
        public VideoMetricsDto Metrics { get; private set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? PublishDate { get; set; }
    }
}