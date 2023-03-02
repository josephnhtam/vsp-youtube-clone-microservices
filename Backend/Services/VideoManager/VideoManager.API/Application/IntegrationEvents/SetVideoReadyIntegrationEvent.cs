using EventBus;
using VideoManager.API.Application.DtoModels;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.IntegrationEvents {
    public class SetVideoReadyIntegrationEvent : IntegrationEvent<SetVideoReadyIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public VideoVisibility Visibility { get; set; }
        public bool AllowedToPublish { get; set; }
        public List<ProcessedVideoDto> Videos { get; set; }
        public long InfoVersion { get; set; }

        public SetVideoReadyIntegrationEvent () { }

        public SetVideoReadyIntegrationEvent (Guid videoId, string title, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, VideoVisibility visibility, bool allowedToPublish, List<ProcessedVideoDto> videos, long infoVersion) {
            VideoId = videoId;
            Title = title;
            Description = description;
            Tags = tags;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            Visibility = visibility;
            AllowedToPublish = allowedToPublish;
            Videos = videos;
            InfoVersion = infoVersion;
        }
    }

    public class SetVideoReadyIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
