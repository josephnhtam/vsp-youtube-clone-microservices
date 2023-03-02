using EventBus;
using VideoManager.SignalRHub.DtoModels;

namespace VideoManager.SignalRHub.IntegrationEvents {
    public class NotifyProcessedVideoAddedIntegrationEvent : IntegrationEvent<NotifyProcessedVideoAddedIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public ProcessedVideoDto Video { get; set; }

        public NotifyProcessedVideoAddedIntegrationEvent (Guid videoId, string creatorId, ProcessedVideoDto video) {
            VideoId = videoId;
            CreatorId = creatorId;
            Video = video;
        }
    }

    public class NotifyProcessedVideoAddedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
