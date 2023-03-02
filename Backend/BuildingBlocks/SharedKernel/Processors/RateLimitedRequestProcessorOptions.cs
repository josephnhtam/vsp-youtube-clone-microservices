namespace SharedKernel.Processors {
    public class RateLimitedRequestProcessorOptions {
        public int MaxConcurrentProcessingLimit { get; set; } = 4;
        public float MaxProcessingRateLimit { get; set; } = 50;
    }
}
