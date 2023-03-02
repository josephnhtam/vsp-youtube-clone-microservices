using EventBus;
using VideoManager.SignalRHub.DtoModels;

namespace VideoManager.SignalRHub.IntegrationEvents {
    public class NotifyVideoProcessedIntegrationEvent : IntegrationEvent<NotifyVideoProcessedIntegrationEventTopic> {
        public VideoDto Video { get; set; }

        public NotifyVideoProcessedIntegrationEvent (VideoDto video) {
            Video = video;
        }
    }

    public class NotifyVideoProcessedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
