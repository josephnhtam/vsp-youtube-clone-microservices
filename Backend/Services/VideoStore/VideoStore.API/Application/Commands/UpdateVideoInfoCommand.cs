using Application.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.Commands {
    public class UpdateVideoInfoCommand : ICommand {
        public Guid VideoId { get; set; }
        public string Titile { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public VideoVisibility Visibility { get; set; }
        public bool AllowedToPublish { get; set; }
        public long InfoVersion { get; set; }

        public UpdateVideoInfoCommand (Guid videoId, string titile, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, VideoVisibility visibility, bool allowedToPublish, long infoVersion) {
            VideoId = videoId;
            Titile = titile;
            Description = description;
            Tags = tags;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            Visibility = visibility;
            AllowedToPublish = allowedToPublish;
            InfoVersion = infoVersion;
        }
    }
}
