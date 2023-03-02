namespace Storage.API.Application.Configurations {
    public class CleanupConfiguration {
        public float DefaultFileRemovalDelayHours { get; set; } = 12;
        public int MaxConcurrentProcessingtLimit { get; set; } = 1;
        public float MaxProcessingRateLimit { get; set; } = 5;
        public int FetchCount { get; set; } = 5;
        public float RetryDelaySeconds { get; set; } = 600;
        public int MaxRetryCount { get; set; } = 10;
    }
}
