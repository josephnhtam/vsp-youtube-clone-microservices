using Application.Contracts;
using Search.API.Application.DtoModels;

namespace Search.API.Application.Commands {
    public class CreateOrUpdateVideoSearchInfoCommand : ICommand {
        public Guid VideoId { get; set; }
        public InternalUserProfileDto CreatorProfile { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public List<string> Tags { get; set; }
        public int? LengthSeconds { get; set; }
        public VideoMetricsDto Metrics { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public long Version { get; set; }

        public CreateOrUpdateVideoSearchInfoCommand (Guid videoId, InternalUserProfileDto creatorProfile, string title, string description, string? thumbnailUrl, string? previewThumbnailUrl, List<string> tags, int? lengthSeconds, VideoMetricsDto metrics, DateTimeOffset createDate, long version) {
            VideoId = videoId;
            CreatorProfile = creatorProfile;
            Title = title;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            LengthSeconds = lengthSeconds;
            Tags = tags;
            Metrics = metrics;
            CreateDate = createDate;
            Version = version;
        }
    }
}
