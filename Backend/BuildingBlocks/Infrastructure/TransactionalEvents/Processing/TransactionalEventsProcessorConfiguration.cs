namespace Infrastructure.TransactionalEvents.Processing {
    public class TransactionalEventsProcessorConfiguration {
        public int MaxConcurrentProcessingtLimit { get; set; } = 5;
        public float MaxProcessingRateLimit { get; set; } = 10;
        public int FetchCount { get; set; } = 200;
        public float RetryDelaySeconds { get; set; } = 5f;
    }
}
