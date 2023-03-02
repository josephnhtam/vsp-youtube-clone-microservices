using VideoManager.SignalRHub.DtoModels;

namespace VideoManager.SignalRHub.Hubs {
    public interface IVideoManagerHubClient {
        Task NotifyVideoUploaded (Guid videoId, string originalFileName, string videoFileUrl);
        Task NotifyVideoRegistered (Guid videoId);
        Task NotifyVideoBeingProcessed (Guid videoId);
        Task NotifyVideoProcesssingFailed (Guid videoId);
        Task NotifyVideoProcessingComplete (VideoDto video);
        Task NotifyProcessedVideoAdded (Guid videoId, ProcessedVideoDto video);
        Task NotifyVideoThumbnailsAdded (Guid videoId, IEnumerable<VideoThumbnailDto> thumbnails);
    }
}
