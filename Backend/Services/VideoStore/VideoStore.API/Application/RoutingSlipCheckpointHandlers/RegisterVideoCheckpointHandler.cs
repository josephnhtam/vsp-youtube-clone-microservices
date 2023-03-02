using EventBus;
using EventBus.Helper.RoutingSlips;
using EventBus.Helper.RoutingSlips.Contracts;
using MediatR;
using VideoStore.API.Application.Commands;
using VideoStore.API.Application.DtoModels;

namespace VideoStore.API.Application.RoutingSlipCheckpointHandlers {
    public class RegisterVideoCheckpointHandler :
        RoutingSlipCheckpointHandler<VideoRegistartionPropertiesDto, RegisterVideoRoutingSlipEventQueue> {

        private readonly IMediator _mediator;

        public RegisterVideoCheckpointHandler (IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider) {
            _mediator = mediator;
        }

        protected override async Task<IRoutingSlipProceedResult> HandleProceed (
            VideoRegistartionPropertiesDto properties,
            IRoutingSlipCheckpointProceedContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new RegisterVideoCommand(
                properties.VideoId,
                properties.CreatorProfile,
                properties.Title,
                properties.Description,
                properties.Tags,
                properties.Visibility,
                properties.CreateDate
            ));

            return context.Complete();
        }

        protected override async Task<IRoutingSlipRollbackResult> HandleRollback (
            VideoRegistartionPropertiesDto properties,
            IRoutingSlipCheckpointRollbackContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new UnregisterVideoCommand(properties.VideoId));

            return context.Complete();
        }

    }

    public class RegisterVideoRoutingSlipEventQueue : RoutingSlipEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoStore.VideoRegistration";
        }
    }
}
