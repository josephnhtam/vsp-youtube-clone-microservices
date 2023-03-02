using EventBus;
using MediatR;
using VideoStore.API.Application.Commands;
using VideoStore.API.Application.IntegrationEvents.VideoManager;

namespace VideoStore.API.Application.IntegrationEventHandlers.VideoManager {
    public class VideoInfoUpdatedIntegrationEventHandler : IntegrationEventHandler<
        VideoInfoUpdatedIntegrationEvent,
        VideoInfoUpdatedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoInfoUpdatedIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoInfoUpdatedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UpdateVideoInfoCommand(
                    integrationEvent.VideoId,
                    integrationEvent.Title,
                    integrationEvent.Description,
                    integrationEvent.Tags,
                    integrationEvent.ThumbnailUrl,
                    integrationEvent.PreviewThumbnailUrl,
                    integrationEvent.Visibility,
                    integrationEvent.AllowedToPublish,
                    integrationEvent.InfoVersion
                ));
        }

    }

    public class VideoInfoUpdatedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoStore." + properties.QueueName;
        }
    }
}
