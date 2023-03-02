using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.VideoProcessor;

namespace VideoManager.API.Application.IntegrationEventHandlers.VideoProcessor {
    public class VideoProcessingStartedIntegrationEventHandler : IntegrationEventHandler<
        VideoProcessingStartedIntegrationEvent,
        VideoProcessingStartedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoProcessingStartedIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoProcessingStartedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new SetVideoBeingProcssedStatusCommand(integrationEvent.VideoId));
        }
    }

    public class VideoProcessingStartedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
