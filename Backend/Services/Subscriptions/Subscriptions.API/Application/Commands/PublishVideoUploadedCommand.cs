using Application.Contracts;

namespace Subscriptions.API.Application.Commands {
    public class PublishVideoUploadedCommand : ICommand {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string? ThumbnailUrl { get; set; }

        public PublishVideoUploadedCommand (Guid videoId, string creatorId, string title, string? thumbnailUrl) {
            VideoId = videoId;
            CreatorId = creatorId;
            Title = title;
            ThumbnailUrl = thumbnailUrl;
        }
    }
}
