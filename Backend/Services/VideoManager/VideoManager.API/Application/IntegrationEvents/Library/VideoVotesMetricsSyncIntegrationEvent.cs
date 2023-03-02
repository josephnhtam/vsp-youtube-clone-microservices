using EventBus;
using EventBus.RabbitMQ;
using RabbitMQ.Client;

namespace VideoManager.API.Application.IntegrationEvents.Library {
    public class VideoVotesMetricsSyncIntegrationEvent : IntegrationEvent<VideoVotesMetricsSyncIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public long LikesCount { get; set; }
        public long DislikesCount { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public VideoVotesMetricsSyncIntegrationEvent (Guid videoId, long likesCount, long dislikesCount, DateTimeOffset updateDate) {
            VideoId = videoId;
            LikesCount = likesCount;
            DislikesCount = dislikesCount;
            UpdateDate = updateDate;
        }
    }

    public class VideoVotesMetricsSyncIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Library." + properties.TopicName;

            if (properties is RabbitMQIntegrationEventTopicProperties rmq) {
                rmq.ExchangeType = ExchangeType.Headers;
            }
        }
    }
}
