using Community.API.Application.Commands;
using Community.API.Application.IntegrationEvents.VideoManager;
using EventBus;
using EventBus.Helper.Idempotency;
using MediatR;

namespace Community.API.Application.IntegrationEventHandlers.Users {
    public class UnregisterVideoIntegrationEventHandler :
        IdempotentIntegrationEventHandler<UnregisterVideoIntegrationEvent, UnregisterVideoIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public UnregisterVideoIntegrationEventHandler (IMediator mediator, IServiceProvider serviceProvider) : base(serviceProvider) {
            _mediator = mediator;
        }

        public override async Task HandleIdempotently (UnregisterVideoIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UnregisterVideoForumCommand(integrationEvent.VideoId));
        }

    }

    public class UnregisterVideoIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Community." + properties.QueueName;
        }
    }
}
