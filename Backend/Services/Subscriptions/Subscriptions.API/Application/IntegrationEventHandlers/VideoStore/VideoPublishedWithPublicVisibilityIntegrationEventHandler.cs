using EventBus;
using MediatR;
using Subscriptions.API.Application.Commands;
using Subscriptions.API.Application.IntegrationEvents.VideoStore;

namespace Subscriptions.API.Application.IntegrationEventHandlers.VideoStore {
    public class VideoPublishedWithPublicVisibilityIntegrationEventHandler :
        IntegrationEventHandler<VideoPublishedWithPublicVisibilityIntegrationEvent, VideoPublishedWithPublicVisibilityIntegrationEventQueue> {

        private readonly IMediator _mediator;
        private readonly ILogger<VideoPublishedWithPublicVisibilityIntegrationEventHandler> _logger;

        public VideoPublishedWithPublicVisibilityIntegrationEventHandler (
            IMediator mediator, ILogger<VideoPublishedWithPublicVisibilityIntegrationEventHandler> logger) {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task Handle (VideoPublishedWithPublicVisibilityIntegrationEvent integrationEvent,
            IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {

            await _mediator.Send(
                new PublishVideoUploadedCommand(
                    integrationEvent.VideoId,
                    integrationEvent.CreatorId,
                    integrationEvent.Title,
                    integrationEvent.ThumbnailUrl
                )
            );
        }

    }

    public class VideoPublishedWithPublicVisibilityIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Subscriptions." + properties.QueueName;
        }
    }
}
