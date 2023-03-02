using EventBus;

namespace VideoManager.API.Application.IntegrationEvents {
    public class NotifyVideoRegisteredIntegrationEvent : IntegrationEvent<NotifyVideoRegisteredIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }

        public NotifyVideoRegisteredIntegrationEvent () { }

        public NotifyVideoRegisteredIntegrationEvent (Guid videoId, string creatorId) {
            VideoId = videoId;
            CreatorId = creatorId;
        }
    }

    public class NotifyVideoRegisteredIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
