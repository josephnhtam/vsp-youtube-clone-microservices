using Community.API.Application.Commands;
using Community.API.Application.DtoModels;
using EventBus;
using EventBus.Helper.RoutingSlips;
using EventBus.Helper.RoutingSlips.Contracts;
using MediatR;

namespace Community.API.Application.RoutingSlipCheckpointHandlers {
    public class RegisterVideoCheckpointHandler :
        RoutingSlipCheckpointHandler<VideoForumCreationPropertiesDto, RegisterVideoRoutingSlipEventQueue> {

        private readonly IMediator _mediator;

        public RegisterVideoCheckpointHandler (IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider) {
            _mediator = mediator;
        }

        protected override async Task<IRoutingSlipProceedResult> HandleProceed (
            VideoForumCreationPropertiesDto properties,
            IRoutingSlipCheckpointProceedContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new CreateVideoForumCommand(
                properties.VideoId,
                properties.CreatorProfile,
                properties.AllowedToComment
            ));

            return context.Complete();
        }

        protected override async Task<IRoutingSlipRollbackResult> HandleRollback (
            VideoForumCreationPropertiesDto properties,
            IRoutingSlipCheckpointRollbackContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new UnregisterVideoForumCommand(properties.VideoId));

            return context.Complete();
        }

    }

    public class RegisterVideoRoutingSlipEventQueue : RoutingSlipEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Community.VideoRegistration";
        }
    }
}
