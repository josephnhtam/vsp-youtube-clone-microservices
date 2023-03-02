using EventBus;

namespace VideoProcessor.Application.IntegrationEvents.VideoManager {
    public class RequestVideoProcessingIntegrationEvent : IntegrationEvent<RequestVideoProcessingIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public string OriginalFileName { get; set; }
        public string VideoFileUrl { get; set; }

        public RequestVideoProcessingIntegrationEvent () { }

        public RequestVideoProcessingIntegrationEvent (Guid videoId, string creatorId, string originalFileName, string videoFileUrl) {
            VideoId = videoId;
            CreatorId = creatorId;
            OriginalFileName = originalFileName;
            VideoFileUrl = videoFileUrl;
        }
    }

    public class RequestVideoProcessingIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
