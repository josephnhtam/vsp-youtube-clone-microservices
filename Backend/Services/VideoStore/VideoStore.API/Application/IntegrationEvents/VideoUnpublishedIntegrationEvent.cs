using EventBus;

namespace VideoStore.API.Application.IntegrationEvents {
    public class VideoUnpublishedIntegrationEvent : IntegrationEvent<VideoUnpublishedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public DateTimeOffset Date { get; set; }
        public long Version { get; set; }

        public VideoUnpublishedIntegrationEvent () { }

        public VideoUnpublishedIntegrationEvent (Guid videoId, DateTimeOffset date, long version) {
            VideoId = videoId;
            Date = date;
            Version = version;
        }
    }

    public class VideoUnpublishedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoStore." + properties.TopicName;
        }
    }
}
