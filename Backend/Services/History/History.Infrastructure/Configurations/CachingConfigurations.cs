namespace History.Infrastructure.Configurations {
    public class CachingConfigurations {
        public int VideoCacheDurationInSeconds { get; set; } = 600;
        public int UserProfileCacheDurationInSeconds { get; set; } = 600;
    }
}
