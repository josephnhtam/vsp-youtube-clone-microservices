namespace Search.Domain.Models {
    public class UserProfile {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long PrimaryVersion { get; set; }
    }
}
