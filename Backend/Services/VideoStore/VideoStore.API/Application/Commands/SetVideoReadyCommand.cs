using Application.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.Commands {
    public class SetVideoReadyCommand : ICommand {
        public Guid VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public VideoVisibility Visibility { get; set; }
        public bool AllowedToPublish { get; set; }
        public List<ProcessedVideo> Videos { get; set; }
        public DateTimeOffset ReadyDate { get; set; }
        public long InfoVersion { get; set; }

        public SetVideoReadyCommand (Guid videoId, string title, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, VideoVisibility visibility, bool allowedToPublish, List<ProcessedVideo> videos, DateTimeOffset readyDate, long infoVersion) {
            VideoId = videoId;
            Title = title;
            Description = description;
            Tags = tags;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            Visibility = visibility;
            AllowedToPublish = allowedToPublish;
            Videos = videos;
            ReadyDate = readyDate;
            InfoVersion = infoVersion;
        }
    }
}
