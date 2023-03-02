using EventBus;

namespace VideoManager.API.Application.IntegrationEvents.VideoStore {
    public class VideoPublishedIntegrationEvent : IntegrationEvent<VideoPublishedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public long Version { get; set; }

        public VideoPublishedIntegrationEvent () { }

        public VideoPublishedIntegrationEvent (Guid videoId, DateTimeOffset publishDate, long version) {
            VideoId = videoId;
            PublishDate = publishDate;
            Version = version;
        }
    }

    public class VideoPublishedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoStore." + properties.TopicName;
        }
    }
}
