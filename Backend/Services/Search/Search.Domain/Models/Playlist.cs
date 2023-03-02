namespace Search.Domain.Models {
    public class Playlist : SearchableItem {
        public string? ThumbnailUrl { get; set; }
        public PlaylistMetrics Metrics { get; set; }
    }
}
