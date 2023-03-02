using EventBus;
using History.API.Application.Commands;
using History.API.Application.IntegrationEvents.VideoStore;
using MediatR;

namespace History.API.Application.IntegrationEventHandlers.VideoStore {
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
            properties.QueueName = "History." + properties.QueueName;
        }
    }
}
