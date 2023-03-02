using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.VideoProcessor;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.IntegrationEventHandlers.VideoProcessor {
    public class VideoProcessingThumbnailsAddedIntegrationEventHandler : IntegrationEventHandler<
        VideoProcessingThumbnailsAddedIntegrationEvent,
        VideoProcessingThumbnailsAddedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoProcessingThumbnailsAddedIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoProcessingThumbnailsAddedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new AddVideoThumbnailsCommand(
                integrationEvent.VideoId,
                integrationEvent.Thumbnails.Select(x =>
                    VideoThumbnail.Create(x.ImageFileId, x.Label, x.Width, x.Height, x.Url)).ToList(),
                VideoPreviewThumbnail.Create(
                    integrationEvent.PreviewThumbnail.ImageFileId,
                    integrationEvent.PreviewThumbnail.Width,
                    integrationEvent.PreviewThumbnail.Height,
                    integrationEvent.PreviewThumbnail.LengthSeconds,
                    integrationEvent.PreviewThumbnail.Url)
                ));
        }
    }

    public class VideoProcessingThumbnailsAddedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
