using Application.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands {
    public class AddVideoThumbnailsCommand : ICommand {
        public Guid VideoId { get; set; }
        public List<VideoThumbnail> Thumbnails { get; set; }
        public VideoPreviewThumbnail PreviewThumbnail { get; set; }

        public AddVideoThumbnailsCommand (Guid videoId, List<VideoThumbnail> thumbnails, VideoPreviewThumbnail previewThumbnail) {
            VideoId = videoId;
            Thumbnails = thumbnails;
            PreviewThumbnail = previewThumbnail;
        }
    }
}
