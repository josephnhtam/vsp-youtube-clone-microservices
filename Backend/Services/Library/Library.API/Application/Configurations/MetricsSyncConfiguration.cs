namespace Library.API.Application.Configurations {
    public class MetricsSyncConfiguration {
        public int SyncDelaySeconds { get; set; }
        public int MaxConcurrentProcessingtLimit { get; set; } = 4;
        public float MaxProcessingRateLimit { get; set; } = 5;
        public int FetchCount { get; set; } = 200;

        public TimeSpan SyncDelay => TimeSpan.FromSeconds(SyncDelaySeconds);
    }
}
