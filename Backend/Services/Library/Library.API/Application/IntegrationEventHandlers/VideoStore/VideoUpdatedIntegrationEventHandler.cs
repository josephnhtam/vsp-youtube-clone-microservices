using EventBus;
using Library.API.Application.Commands;
using Library.API.Application.IntegrationEvents.VideoStore;
using MediatR;

namespace Library.API.Application.IntegrationEventHandlers.VideoStore {
    public class VideoUpdatedIntegrationEventHandler : IntegrationEventHandler<
        VideoUpdatedIntegrationEvent,
        VideoUpdatedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoUpdatedIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoUpdatedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UpdateVideoCommand(
                    integrationEvent.VideoId,
                    integrationEvent.Title,
                    integrationEvent.Description,
                    integrationEvent.Tags,
                    integrationEvent.ThumbnailUrl,
                    integrationEvent.PreviewThumbnailUrl,
                    integrationEvent.LengthSeconds,
                    integrationEvent.Visibility,
                    integrationEvent.Status,
                    integrationEvent.StatusUpdateDate,
                    integrationEvent.Version
                ));
        }

    }

    public class VideoUpdatedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Library." + properties.QueueName;
        }
    }
}
