namespace VideoManager.Domain.Models {
    public enum VideoProcessingStatus {
        WaitingForUserUpload = 0,
        VideoUploaded = 1,
        VideoBeingProcssed = 2,
        VideoProcessed = 3,
        VideoProcessingFailed = 4
    }
}
