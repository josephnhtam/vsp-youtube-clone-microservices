using EventBus;

namespace VideoManager.API.Application.IntegrationEvents {
    public class NotifyVideoUploadedIntegrationEvent : IntegrationEvent<NotifyVideoUploadedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public string OriginalFileName { get; set; }
        public string VideoFileUrl { get; set; }

        public NotifyVideoUploadedIntegrationEvent () { }

        public NotifyVideoUploadedIntegrationEvent (Guid videoId, string creatorId, string originalFileName, string videoFileUrl) {
            VideoId = videoId;
            CreatorId = creatorId;
            OriginalFileName = originalFileName;
            VideoFileUrl = videoFileUrl;
        }
    }

    public class NotifyVideoUploadedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
