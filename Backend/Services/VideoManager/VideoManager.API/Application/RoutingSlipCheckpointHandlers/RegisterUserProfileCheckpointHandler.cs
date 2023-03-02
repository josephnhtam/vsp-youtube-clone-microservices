using EventBus;
using EventBus.Helper.RoutingSlips;
using EventBus.Helper.RoutingSlips.Contracts;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.RoutingSlipCheckpointHandlers {
    public class RegisterUserProfileCheckpointHandler :
        RoutingSlipCheckpointHandler<InternalUserProfileDto, RegisterUserProfileRoutingSlipEventQueue> {

        private readonly IMediator _mediator;

        public RegisterUserProfileCheckpointHandler (IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider) {
            _mediator = mediator;
        }

        protected override async Task<IRoutingSlipProceedResult> HandleProceed (
            InternalUserProfileDto properties,
            IRoutingSlipCheckpointProceedContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new CreateOrUpdateUserProfileCommand(
                properties.Id,
                properties.DisplayName,
                properties.Handle,
                properties.ThumbnailUrl,
                properties.Version,
                true
            ));

            return context.Complete();
        }

        protected override async Task<IRoutingSlipRollbackResult> HandleRollback (
            InternalUserProfileDto properties,
            IRoutingSlipCheckpointRollbackContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new DeleteUserProfileCommand(properties.Id));

            return context.Complete();
        }

    }

    public class RegisterUserProfileRoutingSlipEventQueue : RoutingSlipEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.UserProfileRegistration";
        }
    }
}
