using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.VideoStore;

namespace VideoManager.API.Application.IntegrationEventHandlers.VideoStore {
    public class VideoPublishedIntegrationEventHandler : IntegrationEventHandler<
        VideoPublishedIntegrationEvent,
        VideoPublishedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoPublishedIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoPublishedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new SetVideoPublishStatusCommand(integrationEvent.VideoId, true, integrationEvent.PublishDate, integrationEvent.Version));
        }
    }

    public class VideoPublishedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
