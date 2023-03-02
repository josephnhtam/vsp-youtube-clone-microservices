using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.VideoProcessor;

namespace VideoManager.API.Application.IntegrationEventHandlers.VideoProcessor {
    public class VideoProcessingFailedIntegrationEventHandler : IntegrationEventHandler<
        VideoProcessingFailedIntegrationEvent,
        VideoProcessingFailedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoProcessingFailedIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoProcessingFailedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new SetVideoProcessingFailedStatusCommand(integrationEvent.VideoId));
        }
    }

    public class VideoProcessingFailedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
