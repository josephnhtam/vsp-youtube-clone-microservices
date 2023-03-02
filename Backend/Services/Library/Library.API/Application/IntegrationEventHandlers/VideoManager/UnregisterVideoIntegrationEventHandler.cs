using EventBus;
using EventBus.Helper.Idempotency;
using Library.API.Application.Commands;
using Library.API.Application.IntegrationEvents.VideoManager;
using MediatR;

namespace Library.API.Application.IntegrationEventHandlers.VideoManager {
    public class UnregisterVideoIntegrationEventHandler :
        IdempotentIntegrationEventHandler<UnregisterVideoIntegrationEvent, UnregisterVideoIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public UnregisterVideoIntegrationEventHandler (IMediator mediator, IServiceProvider serviceProvider) : base(serviceProvider) {
            _mediator = mediator;
        }

        public override async Task HandleIdempotently (UnregisterVideoIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UnregisterVideoCommand(integrationEvent.VideoId));
        }

    }

    public class UnregisterVideoIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Library." + properties.QueueName;
        }
    }
}
