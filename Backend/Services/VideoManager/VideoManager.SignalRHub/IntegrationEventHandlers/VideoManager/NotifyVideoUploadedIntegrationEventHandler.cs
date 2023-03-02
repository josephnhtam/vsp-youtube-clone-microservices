using EventBus;
using Microsoft.AspNetCore.SignalR;
using VideoManager.SignalRHub.Hubs;
using VideoManager.SignalRHub.IntegrationEvents;

namespace VideoManager.SignalRHub.IntegrationEventHandlers.VideoManager {
    public class NotifyVideoUploadedIntegrationEventHandler : IntegrationEventHandler<
        NotifyVideoUploadedIntegrationEvent,
        NotifyVideoUploadedIntegrationEventQueue> {

        private readonly IHubContext<VideoManagerHub, IVideoManagerHubClient> _hubContext;

        public NotifyVideoUploadedIntegrationEventHandler (IHubContext<VideoManagerHub, IVideoManagerHubClient> hubContext) {
            _hubContext = hubContext;
        }

        public override async Task Handle (NotifyVideoUploadedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _hubContext.Clients.User(integrationEvent.CreatorId).NotifyVideoUploaded(integrationEvent.VideoId, integrationEvent.OriginalFileName, integrationEvent.VideoFileUrl);
        }

    }

    public class NotifyVideoUploadedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.SignalRHub." + properties.QueueName;
        }
    }
}
