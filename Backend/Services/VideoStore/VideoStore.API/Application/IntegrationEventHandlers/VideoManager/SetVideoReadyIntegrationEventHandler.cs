using EventBus;
using MediatR;
using VideoStore.API.Application.Commands;
using VideoStore.API.Application.IntegrationEvents.VideoManager;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.IntegrationEventHandlers.VideoManager {
    public class SetVideoReadyIntegrationEventHandler : IntegrationEventHandler<
        SetVideoReadyIntegrationEvent,
        SetVideoReadyIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public SetVideoReadyIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (SetVideoReadyIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            var videos = integrationEvent.Videos.Select(x =>
                ProcessedVideo.Create(x.VideoFileId, x.Label, x.Width, x.Height, x.Size, x.LengthSeconds, x.Url)).ToList();

            await _mediator.Send(new SetVideoReadyCommand(
                    integrationEvent.VideoId,
                    integrationEvent.Title,
                    integrationEvent.Description,
                    integrationEvent.Tags,
                    integrationEvent.ThumbnailUrl,
                    integrationEvent.PreviewThumbnailUrl,
                    integrationEvent.Visibility,
                    integrationEvent.AllowedToPublish,
                    videos,
                    integrationEvent.CreateDate,
                    integrationEvent.InfoVersion
                ));
        }

    }

    public class SetVideoReadyIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoStore." + properties.QueueName;
        }
    }
}
