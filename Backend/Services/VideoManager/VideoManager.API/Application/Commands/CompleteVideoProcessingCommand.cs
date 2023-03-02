using Application.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands {
    public class CompleteVideoProcessingCommand : ICommand {
        public Guid VideoId { get; set; }
        public List<VideoThumbnail> Thumbnails { get; set; }
        public VideoPreviewThumbnail? PreviewThumbnail { get; set; }
        public List<ProcessedVideo> Videos { get; set; }

        public CompleteVideoProcessingCommand (Guid videoId, List<VideoThumbnail> thumbnails, VideoPreviewThumbnail? previewThumbnail, List<ProcessedVideo> videos) {
            VideoId = videoId;
            Thumbnails = thumbnails;
            PreviewThumbnail = previewThumbnail;
            Videos = videos;
        }
    }
}
