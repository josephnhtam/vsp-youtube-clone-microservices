using EventBus;
using EventBus.Helper.RoutingSlips;
using EventBus.Helper.RoutingSlips.Contracts;
using MediatR;
using Users.API.Application.Commands;
using Users.API.Application.DtoModels;

namespace Users.API.Application.RoutingSlipCheckpointHandlers {
    public class UserProfileRegistrationFailedCheckpointHandler :
        RoutingSlipCheckpointHandler<InternalUserProfileDto, UserProfileRegistrationFailedRoutingSlipEventQueue> {

        private readonly IMediator _mediator;

        public UserProfileRegistrationFailedCheckpointHandler (IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider) {
            _mediator = mediator;
        }

        protected override async Task<IRoutingSlipRollbackResult> HandleRollback (
            InternalUserProfileDto properties,
            IRoutingSlipCheckpointRollbackContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new FailUserProfileRegistrationCommand(properties.Id));
            return context.Complete();
        }

    }

    public class UserProfileRegistrationFailedRoutingSlipEventQueue : RoutingSlipEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Users.UserProfileRegistrationFailed";
        }
    }
}
