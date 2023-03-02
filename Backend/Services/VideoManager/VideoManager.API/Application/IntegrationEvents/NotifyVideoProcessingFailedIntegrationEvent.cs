using EventBus;

namespace VideoManager.API.Application.IntegrationEvents {
    public class NotifyVideoProcessingFailedIntegrationEvent : IntegrationEvent<NotifyVideoProcessingFailedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }

        public NotifyVideoProcessingFailedIntegrationEvent () { }

        public NotifyVideoProcessingFailedIntegrationEvent (Guid videoId, string creatorId) {
            VideoId = videoId;
            CreatorId = creatorId;
        }
    }

    public class NotifyVideoProcessingFailedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
