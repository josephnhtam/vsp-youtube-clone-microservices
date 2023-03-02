using EventBus;
using MediatR;
using Storage.API.Application.Commands;
using Storage.Shared.IntegrationEvents;

namespace Storage.API.Application.IntegrationEventHandlers {
    public class SetFileInUseIntegrationEventHandler : IntegrationEventHandler<SetFileInUseIntegrationEvent, SetFileInUseIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public SetFileInUseIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (SetFileInUseIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new SetFileInUseCommand(integrationEvent.FileIds));
        }

    }

    public class SetFileInUseIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Storage." + properties.QueueName;
        }
    }
}
