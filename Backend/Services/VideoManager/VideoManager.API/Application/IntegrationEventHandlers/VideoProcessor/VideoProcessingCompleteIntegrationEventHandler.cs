using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.VideoProcessor;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.IntegrationEventHandlers.VideoProcessor {
    public class VideoProcessingCompleteIntegrationEventHandler : IntegrationEventHandler<
        VideoProcessingCompleteIntegrationEvent,
        VideoProcessingCompleteIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoProcessingCompleteIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoProcessingCompleteIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            var previewThumbnail = integrationEvent.PreviewThumbnail == null ? null :
                VideoPreviewThumbnail.Create(
                    integrationEvent.PreviewThumbnail.ImageFileId,
                    integrationEvent.PreviewThumbnail.Width,
                    integrationEvent.PreviewThumbnail.Height,
                    integrationEvent.PreviewThumbnail.LengthSeconds,
                    integrationEvent.PreviewThumbnail.Url
                );

            await _mediator.Send(new CompleteVideoProcessingCommand(
                    integrationEvent.VideoId,
                    integrationEvent.Thumbnails.Select(x =>
                        VideoThumbnail.Create(x.ImageFileId, x.Label, x.Width, x.Height, x.Url)).ToList(),
                    previewThumbnail,
                    integrationEvent.Videos.Select(x =>
                        ProcessedVideo.Create(x.VideoFileId, x.Label, x.Width, x.Height, x.Size, x.LengthSeconds, x.Url)).ToList()
                    ));
        }
    }

    public class VideoProcessingCompleteIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
