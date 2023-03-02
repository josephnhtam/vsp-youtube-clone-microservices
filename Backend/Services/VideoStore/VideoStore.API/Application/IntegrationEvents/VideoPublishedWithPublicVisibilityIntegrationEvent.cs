using EventBus;

namespace VideoStore.API.Application.IntegrationEvents {
    public class VideoPublishedWithPublicVisibilityIntegrationEvent : IntegrationEvent<VideoPublishedWithPublicVisibilityIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public int? LengthSeconds { get; set; }
        public DateTimeOffset? StatusUpdateDate { get; set; }
        public long Version { get; set; }

        public VideoPublishedWithPublicVisibilityIntegrationEvent () { }

        public VideoPublishedWithPublicVisibilityIntegrationEvent (Guid videoId, string creatorId, string title, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, int? lengthSeconds, DateTimeOffset? statusUpdateDate, long version) {
            VideoId = videoId;
            CreatorId = creatorId;
            Title = title;
            Description = description;
            Tags = tags;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            LengthSeconds = lengthSeconds;
            StatusUpdateDate = statusUpdateDate;
            Version = version;
        }
    }

    public class VideoPublishedWithPublicVisibilityIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoStore." + properties.TopicName;
        }
    }
}
