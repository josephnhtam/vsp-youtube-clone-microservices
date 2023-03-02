using EventBus;
using MediatR;
using Storage.API.Application.Commands;
using Storage.Shared.IntegrationEvents;

namespace Storage.API.Application.IntegrationEventHandlers {
    public class RemoveFileIntegrationEventHandler : IntegrationEventHandler<RemoveFileIntegrationEvent, RemoveFileIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public RemoveFileIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (RemoveFileIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new RemoveFileCommand(integrationEvent.FileIds, integrationEvent.RemovalDelay));
        }

    }

    public class RemoveFileIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Storage." + properties.QueueName;
        }
    }
}
