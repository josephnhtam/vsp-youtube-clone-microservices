using EventBus;
using Microsoft.AspNetCore.SignalR;
using VideoManager.SignalRHub.Hubs;
using VideoManager.SignalRHub.IntegrationEvents;

namespace VideoManager.SignalRHub.IntegrationEventHandlers.VideoManager {
    public class NotifyProcessedVideoAddedIntegrationEventHandler : IntegrationEventHandler<
        NotifyProcessedVideoAddedIntegrationEvent,
        NotifyProcessedVideoAddedIntegrationEventQueue> {

        private readonly IHubContext<VideoManagerHub, IVideoManagerHubClient> _hubContext;

        public NotifyProcessedVideoAddedIntegrationEventHandler (IHubContext<VideoManagerHub, IVideoManagerHubClient> hubContext) {
            _hubContext = hubContext;
        }

        public override async Task Handle (NotifyProcessedVideoAddedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _hubContext.Clients.User(integrationEvent.CreatorId)
                .NotifyProcessedVideoAdded(integrationEvent.VideoId, integrationEvent.Video);
        }

    }

    public class NotifyProcessedVideoAddedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.SignalRHub." + properties.QueueName;
        }
    }
}
