using EventBus;
using EventBus.Helper.RoutingSlips;
using EventBus.Helper.RoutingSlips.Contracts;
using MediatR;
using Users.API.Application.Commands;
using Users.API.Application.DtoModels;

namespace Users.API.Application.RoutingSlipCheckpointHandlers {
    public class UserProfileRegisteredCheckpointHandler :
        RoutingSlipCheckpointHandler<InternalUserProfileDto, UserProfileRegisteredRoutingSlipEventQueue> {

        private readonly IMediator _mediator;

        public UserProfileRegisteredCheckpointHandler (IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider) {
            _mediator = mediator;
        }

        protected override async Task<IRoutingSlipProceedResult> HandleProceed (
            InternalUserProfileDto properties,
            IRoutingSlipCheckpointProceedContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new CompleteUserProfileRegistrationCommand(properties.Id));
            return context.Complete();
        }

    }

    public class UserProfileRegisteredRoutingSlipEventQueue : RoutingSlipEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Users.UserProfileRegistered";
        }
    }
}
