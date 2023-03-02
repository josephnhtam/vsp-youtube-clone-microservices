using EventBus;
using EventBus.Helper.RoutingSlips;
using EventBus.Helper.RoutingSlips.Contracts;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.RoutingSlipCheckpointHandlers {
    public class VideoRegisteredCheckpointHandler :
        RoutingSlipCheckpointHandler<VideoRegistartionPropertiesDto, VideoRegisteredRoutingSlipEventQueue> {

        private readonly IMediator _mediator;

        public VideoRegisteredCheckpointHandler (IServiceProvider serviceProvider, IMediator mediator) : base(serviceProvider) {
            _mediator = mediator;
        }

        protected override async Task<IRoutingSlipProceedResult> HandleProceed (
            VideoRegistartionPropertiesDto properties,
            IRoutingSlipCheckpointProceedContext context,
            IIncomingIntegrationEventProperties eventProperties,
            IIncomingIntegrationEventContext eventContext) {

            await _mediator.Send(new SetVideoRegisteredStatusCommand(properties.VideoId));
            return context.Complete();
        }

    }

    public class VideoRegisteredRoutingSlipEventQueue : RoutingSlipEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager.VideoRegistered";
        }
    }
}
