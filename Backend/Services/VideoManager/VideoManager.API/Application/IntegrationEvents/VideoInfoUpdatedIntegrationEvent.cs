using EventBus;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.IntegrationEvents {
    public class VideoInfoUpdatedIntegrationEvent : IntegrationEvent<VideoInfoUpdatedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public VideoVisibility Visibility { get; set; }
        public bool AllowedToPublish { get; set; }
        public long InfoVersion { get; set; }

        public VideoInfoUpdatedIntegrationEvent () { }

        public VideoInfoUpdatedIntegrationEvent (Guid videoId, string title, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, VideoVisibility visibility, bool allowedToPublish, long infoVersion) {
            VideoId = videoId;
            Title = title;
            Description = description;
            Tags = tags;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            Visibility = visibility;
            AllowedToPublish = allowedToPublish;
            InfoVersion = infoVersion;
        }
    }

    public class VideoInfoUpdatedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
