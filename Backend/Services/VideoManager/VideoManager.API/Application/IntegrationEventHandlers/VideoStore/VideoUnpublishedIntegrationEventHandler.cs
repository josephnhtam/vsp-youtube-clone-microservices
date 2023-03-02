using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.VideoStore;

namespace VideoManager.API.Application.IntegrationEventHandlers.VideoStore {
    public class VideoUnpublishedIntegrationEventHandler : IntegrationEventHandler<
        VideoUnpublishedIntegrationEvent,
        VideoUnpublishedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoUnpublishedIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoUnpublishedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new SetVideoPublishStatusCommand(integrationEvent.VideoId, false, integrationEvent.Date, integrationEvent.Version));
        }
    }

    public class VideoUnpublishedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
