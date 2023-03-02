using EventBus;
using Microsoft.AspNetCore.SignalR;
using VideoManager.SignalRHub.Hubs;
using VideoManager.SignalRHub.IntegrationEvents;

namespace VideoManager.SignalRHub.IntegrationEventHandlers.VideoManager {
    public class NotifyVideoProcessedIntegrationEventHandler : IntegrationEventHandler<
        NotifyVideoProcessedIntegrationEvent,
        NotifyVideoProcessedIntegrationEventQueue> {

        private readonly IHubContext<VideoManagerHub, IVideoManagerHubClient> _hubContext;

        public NotifyVideoProcessedIntegrationEventHandler (IHubContext<VideoManagerHub, IVideoManagerHubClient> hubContext) {
            _hubContext = hubContext;
        }

        public override async Task Handle (NotifyVideoProcessedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _hubContext.Clients.User(integrationEvent.Video.CreatorId)
                .NotifyVideoProcessingComplete(integrationEvent.Video);
        }

    }

    public class NotifyVideoProcessedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.SignalRHub." + properties.QueueName;
        }
    }
}
