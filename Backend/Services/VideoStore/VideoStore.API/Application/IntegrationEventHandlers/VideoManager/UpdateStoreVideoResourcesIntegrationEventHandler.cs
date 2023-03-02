using EventBus;
using MediatR;
using VideoStore.API.Application.Commands;
using VideoStore.API.Application.IntegrationEvents.VideoManager;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.IntegrationEventHandlers.VideoManager {
    public class UpdateStoreVideoResourcesIntegrationEventHandler : IntegrationEventHandler<
        UpdateStoreVideoResourcesIntegrationEvent,
        UpdateStoreVideoResourcesIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public UpdateStoreVideoResourcesIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (UpdateStoreVideoResourcesIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            var videos = integrationEvent.Videos?.Select(x =>
                ProcessedVideo.Create(x.VideoFileId, x.Label, x.Width, x.Height, x.Size, x.LengthSeconds, x.Url)).ToList();

            await _mediator.Send(new UpdateVideoResourcesCommand(
                    integrationEvent.VideoId,
                    videos,
                    integrationEvent.Merge,
                    integrationEvent.CreateDate
                ));
        }

    }

    public class UpdateStoreVideoResourcesIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoStore." + properties.QueueName;
        }
    }
}
