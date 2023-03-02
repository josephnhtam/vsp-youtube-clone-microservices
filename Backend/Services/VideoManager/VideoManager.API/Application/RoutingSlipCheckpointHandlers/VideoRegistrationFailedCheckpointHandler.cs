using EventBus;
using EventBus.Helper.RoutingSlips;
using EventBus.Helper.RoutingSlips.Contracts;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.RoutingSlipCheckpointHandlers {
    public class VideoRegistrationFailedCheckpointHandler :
        RoutingSlipCheckpointHandler<VideoRegistartionPropertiesDto, VideoRegistrationFailedRoutingSlipEventQueue> {

        private readonly IMediator _mediator;

        public VideoRegistrationFailedCheckpointHandler (IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider) {
            _mediator = mediator;
        }

        protected override async Task<IRoutingSlipRollbackResult> HandleRollback (
            VideoRegistartionPropertiesDto properties,
            IRoutingSlipCheckpointRollbackContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new SetVideoRegistrationFailedStatusCommand(properties.VideoId));

            return context.Complete();
        }

    }

    public class VideoRegistrationFailedRoutingSlipEventQueue : RoutingSlipEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.RegistrationFailed";
        }
    }
}
