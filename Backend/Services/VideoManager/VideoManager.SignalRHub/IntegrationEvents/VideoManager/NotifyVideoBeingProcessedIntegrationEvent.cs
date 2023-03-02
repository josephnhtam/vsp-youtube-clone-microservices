using EventBus;

namespace VideoManager.SignalRHub.IntegrationEvents {
    public class NotifyVideoBeingProcessedIntegrationEvent : IntegrationEvent<NotifyVideoBeingProcessedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }

        public NotifyVideoBeingProcessedIntegrationEvent () { }

        public NotifyVideoBeingProcessedIntegrationEvent (Guid videoId, string creatorId) {
            VideoId = videoId;
            CreatorId = creatorId;
        }
    }

    public class NotifyVideoBeingProcessedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
