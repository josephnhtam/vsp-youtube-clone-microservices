using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.VideoProcessor;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.IntegrationEventHandlers.VideoProcessor {
    public class VideoRegisteredToStoreIntegrationEventHandler : IntegrationEventHandler<
        VideoProcessingVideoAddedIntegrationEvent,
        VideoProcessingVideoAddedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoRegisteredToStoreIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoProcessingVideoAddedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new AddProcessedVideoCommand(
                integrationEvent.VideoId,
                ProcessedVideo.Create(
                    integrationEvent.Video.VideoFileId,
                    integrationEvent.Video.Label,
                    integrationEvent.Video.Width,
                    integrationEvent.Video.Height,
                    integrationEvent.Video.Size,
                    integrationEvent.Video.LengthSeconds,
                    integrationEvent.Video.Url))
                );
        }
    }

    public class VideoProcessingVideoAddedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
