namespace VideoProcessor.Application.Configurations {
    public class VideoProcessorConfiguration {
        public int MaxConcurrentProcessingLimit { get; set; } = 4;
        public float MaxProcessingRateLimit { get; set; } = 5;
        public float ProcessingLockDurationSeconds { get; set; } = 120;
        public float RetryDelaySeconds { get; set; } = 60;
        public int MaxRetryCount { get; set; } = 10;
        public int PreviewThumbnailHeight { get; set; } = 320;
        public float PreviewThumbnailStartPosition { get; set; } = 0.33f;
        public float PreviewThumbnailLengthSeconds { get; set; } = 3f;
        public int ThumbnailHeight { get; set; } = 360;
        public List<ThumbnailPositionConfiguration> ThumbnailPositions { get; set; }
        public List<VideoProcessingStepConfiguration> VideoProcessingSteps { get; set; }
    }

    public class ThumbnailPositionConfiguration {
        public int? Seconds { get; set; }
        public float? TimePercentage { get; set; }
    }

    public class VideoProcessingStepConfiguration {
        public string Label { get; set; }
        public int Height { get; set; }
    }
}
