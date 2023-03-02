using EventBus;

namespace VideoManager.API.Application.IntegrationEvents.Storage {
    public class VideoUploadedIntegrationEvent : IntegrationEvent<VideoUploadedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public string OriginalFileName { get; set; }
        public string Url { get; set; }
        public DateTimeOffset Date { get; set; }

        public VideoUploadedIntegrationEvent () { }

        public VideoUploadedIntegrationEvent (Guid videoId, string creatorId, string originalFileName, string url) {
            VideoId = videoId;
            CreatorId = creatorId;
            OriginalFileName = originalFileName;
            Url = url;
            Date = DateTimeOffset.UtcNow;
        }
    }

    public class VideoUploadedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Storage." + properties.TopicName;
        }
    }
}
