namespace VideoProcessor.Domain.Models {
    public enum VideoProcessingStatus {
        Pending = 0,
        ProcessingThumbnails = 1,
        ProcessingVideos = 2,
        Processed = 100,
        Failed = 102
    }
}
