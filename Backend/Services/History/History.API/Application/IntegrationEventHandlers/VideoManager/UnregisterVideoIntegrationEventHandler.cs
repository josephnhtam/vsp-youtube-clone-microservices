using EventBus;
using EventBus.Helper.Idempotency;
using History.API.Application.Commands;
using History.API.Application.IntegrationEvents.VideoManager;
using MediatR;

namespace History.API.Application.IntegrationEventHandlers.VideoManager {
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
            properties.QueueName = "History." + properties.QueueName;
        }
    }
}
