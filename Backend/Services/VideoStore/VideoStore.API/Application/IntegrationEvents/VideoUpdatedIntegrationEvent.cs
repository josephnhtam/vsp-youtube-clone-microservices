using EventBus;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.IntegrationEvents {
    public class VideoUpdatedIntegrationEvent : IntegrationEvent<VideoUpdatedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public int? LengthSeconds { get; set; }
        public VideoVisibility Visibility { get; set; }
        public VideoStatus Status { get; set; }
        public DateTimeOffset? StatusUpdateDate { get; set; }
        public long Version { get; set; }

        public VideoUpdatedIntegrationEvent () { }

        public VideoUpdatedIntegrationEvent (Guid videoId, string title, string description, string tags, string? thumbnailUrl, string? previewThumbnailUrl, int? lengthSeconds, VideoVisibility visibility, VideoStatus status, DateTimeOffset? statusUpdateDate, long version) {
            VideoId = videoId;
            Title = title;
            Description = description;
            Tags = tags;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = previewThumbnailUrl;
            LengthSeconds = lengthSeconds;
            Visibility = visibility;
            Status = status;
            StatusUpdateDate = statusUpdateDate;
            Version = version;
        }
    }

    public class VideoUpdatedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoStore." + properties.TopicName;
        }
    }
}
