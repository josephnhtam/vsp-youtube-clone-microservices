using EventBus;
using EventBus.RabbitMQ;
using RabbitMQ.Client;

namespace Community.API.Application.IntegrationEvents {
    public class VideoCommentsMetricsSyncIntegrationEvent : IntegrationEvent<VideoCommentsMetricsSyncIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public long CommentsCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public VideoCommentsMetricsSyncIntegrationEvent (Guid videoId, long commentsCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            CommentsCount = commentsCount;
            UpdateDate = updateDate;
        }
    }

    public class VideoCommentsMetricsSyncIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Community." + properties.TopicName;

            if (properties is RabbitMQIntegrationEventTopicProperties rmq) {
                rmq.ExchangeType = ExchangeType.Headers;
            }
        }
    }
}
