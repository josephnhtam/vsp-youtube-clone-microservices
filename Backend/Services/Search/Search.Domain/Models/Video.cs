namespace Search.Domain.Models {
    public class Video : SearchableItem {
        public string Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public int? LengthSeconds { get; set; }
        public VideoMetrics Metrics { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
