namespace Search.API.Application.DtoModels {
    public class VideoDto : SearchableItemDto {
        public string Type => "video";
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public int? LengthSeconds { get; set; }
        public string Description { get; set; }
        public VideoMetricsDto Metrics { get; private set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
