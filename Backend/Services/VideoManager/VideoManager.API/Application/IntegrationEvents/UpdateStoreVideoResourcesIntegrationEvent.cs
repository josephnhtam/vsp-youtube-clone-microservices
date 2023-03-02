using EventBus;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.IntegrationEvents {
    public class UpdateStoreVideoResourcesIntegrationEvent : IntegrationEvent<UpdateStoreVideoResourcesIntegrationEventTopic> {
        public Guid VideoId { get; set; }
        public List<ProcessedVideoDto> Videos { get; set; }
        public bool Merge { get; set; }

        public UpdateStoreVideoResourcesIntegrationEvent () { }

        public UpdateStoreVideoResourcesIntegrationEvent (Guid videoId, List<ProcessedVideoDto> videos, bool merge) {
            VideoId = videoId;
            Videos = videos;
            Merge = merge;
        }
    }

    public class UpdateStoreVideoResourcesIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "VideoManager." + properties.TopicName;
        }
    }
}
