using EventBus;
using EventBus.RabbitMQ;
using RabbitMQ.Client;

namespace History.API.Application.IntegrationEvents {
    public class VideoViewsMetricsSyncIntegrationEvent : IntegrationEvent<VideoViewsMetricsSyncIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public long ViewsCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public VideoViewsMetricsSyncIntegrationEvent (Guid videoId, long viewsCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            ViewsCount = viewsCount;
            UpdateDate = updateDate;
        }
    }

    public class VideoViewsMetricsSyncIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "History." + properties.TopicName;

            if (properties is RabbitMQIntegrationEventTopicProperties rmq) {
                rmq.ExchangeType = ExchangeType.Headers;
            }
        }
    }
}
