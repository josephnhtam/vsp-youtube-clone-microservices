using EventBus;
using MediatR;
using Search.API.Application.Commands;
using Search.API.Application.IntegrationEvents.Library;

namespace Search.API.Application.IntegrationEventHandlers.Library {
    public class RemoveVideoSearchInfoIntegrationEventHandler : IntegrationEventHandler<
        RemoveVideoSearchInfoIntegrationEvent,
        RemoveVideoSearchInfoIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public RemoveVideoSearchInfoIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (RemoveVideoSearchInfoIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new RemoveVideoSearchInfoCommand(
                integrationEvent.VideoId,
                integrationEvent.Version));
        }
    }

    public class RemoveVideoSearchInfoIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Search." + properties.QueueName;
        }
    }
}
