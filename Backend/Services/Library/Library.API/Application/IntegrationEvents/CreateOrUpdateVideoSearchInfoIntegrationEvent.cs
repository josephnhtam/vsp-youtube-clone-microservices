using EventBus;
using Library.API.Application.DtoModels;

namespace Library.API.Application.IntegrationEvents {
    public class CreateOrUpdateVideoSearchInfoIntegrationEvent : IntegrationEvent<VideoSearchInfoUpdatedIntegrationEventTopic> {
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

        public CreateOrUpdateVideoSearchInfoIntegrationEvent () { }

        public CreateOrUpdateVideoSearchInfoIntegrationEvent (Guid videoId, InternalUserProfileDto creatorProfile, string title, string description, string? thumbnailUrl, string? previewThumbnailUrl, List<string> tags, int? lengthSeconds, VideoMetricsDto metrics, DateTimeOffset createDate, long vesrion) {
            VideoId = videoId;
            CreatorProfile = creatorProfile;
            Title = title;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            Tags = tags;
            LengthSeconds = lengthSeconds;
            Metrics = metrics;
            CreateDate = createDate;
            Version = vesrion;
        }
    }

    public class VideoSearchInfoUpdatedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Library." + properties.TopicName;
        }
    }
}
