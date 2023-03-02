using EventBus;
using MediatR;
using Search.API.Application.Commands;
using Search.API.Application.IntegrationEvents.Library;

namespace Search.API.Application.IntegrationEventHandlers.Library {
    public class CreateOrUpdateVideoSearchInfoIntegrationEventHandler : IntegrationEventHandler<
        CreateOrUpdateVideoSearchInfoIntegrationEvent,
        CreateOrUpdateVideoSearchInfoIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public CreateOrUpdateVideoSearchInfoIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (CreateOrUpdateVideoSearchInfoIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new CreateOrUpdateVideoSearchInfoCommand(
                integrationEvent.VideoId,
                integrationEvent.CreatorProfile,
                integrationEvent.Title,
                integrationEvent.Description,
                integrationEvent.ThumbnailUrl,
                integrationEvent.PreviewThumbnailUrl,
                integrationEvent.Tags,
                integrationEvent.LengthSeconds,
                integrationEvent.Metrics,
                integrationEvent.CreateDate,
                integrationEvent.Version));
        }
    }

    public class CreateOrUpdateVideoSearchInfoIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Search." + properties.QueueName;
        }
    }
}
