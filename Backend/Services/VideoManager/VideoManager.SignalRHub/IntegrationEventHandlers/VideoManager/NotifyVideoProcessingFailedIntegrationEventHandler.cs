using EventBus;
using Microsoft.AspNetCore.SignalR;
using VideoManager.SignalRHub.Hubs;
using VideoManager.SignalRHub.IntegrationEvents;

namespace VideoManager.SignalRHub.IntegrationEventHandlers.VideoManager {
    public class NotifyVideoProcessingFailedIntegrationEventHandler : IntegrationEventHandler<
        NotifyVideoProcessingFailedIntegrationEvent,
        NotifyVideoProcessingFailedIntegrationEventQueue> {

        private readonly IHubContext<VideoManagerHub, IVideoManagerHubClient> _hubContext;

        public NotifyVideoProcessingFailedIntegrationEventHandler (IHubContext<VideoManagerHub, IVideoManagerHubClient> hubContext) {
            _hubContext = hubContext;
        }

        public override async Task Handle (NotifyVideoProcessingFailedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _hubContext.Clients.User(integrationEvent.CreatorId).NotifyVideoProcesssingFailed(integrationEvent.VideoId);
        }

    }

    public class NotifyVideoProcessingFailedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.SignalRHub." + properties.QueueName;
        }
    }
}
