using EventBus;
using Microsoft.AspNetCore.SignalR;
using VideoManager.SignalRHub.Hubs;
using VideoManager.SignalRHub.IntegrationEvents;

namespace VideoManager.SignalRHub.IntegrationEventHandlers.VideoManager {
    public class NotifyVideoBeingProcessedIntegrationEventHandler : IntegrationEventHandler<
        NotifyVideoBeingProcessedIntegrationEvent,
        NotifyVideoBeingProcessedIntegrationEventQueue> {

        private readonly IHubContext<VideoManagerHub, IVideoManagerHubClient> _hubContext;

        public NotifyVideoBeingProcessedIntegrationEventHandler (IHubContext<VideoManagerHub, IVideoManagerHubClient> hubContext) {
            _hubContext = hubContext;
        }

        public override async Task Handle (NotifyVideoBeingProcessedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _hubContext.Clients.User(integrationEvent.CreatorId).NotifyVideoBeingProcessed(integrationEvent.VideoId);
        }

    }

    public class NotifyVideoBeingProcessedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.SignalRHub." + properties.QueueName;
        }
    }
}
