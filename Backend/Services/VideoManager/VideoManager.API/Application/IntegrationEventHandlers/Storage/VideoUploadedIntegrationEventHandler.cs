using EventBus;
using EventBus.Helper.Idempotency;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.Storage;

namespace VideoManager.API.Application.IntegrationEventHandlers.Storage {
    public class VideoUploadedIntegrationEventHandler : IdempotentIntegrationEventHandler<
        VideoUploadedIntegrationEvent,
        VideoUploadedIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoUploadedIntegrationEventHandler (IMediator mediator, IServiceProvider serviceProvider) : base(serviceProvider) {
            _mediator = mediator;
        }

        public override async Task HandleIdempotently (VideoUploadedIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new SetVideoUploadedStatusCommand(
                integrationEvent.VideoId,
                integrationEvent.CreatorId,
                integrationEvent.OriginalFileName,
                integrationEvent.Url,
                integrationEvent.Date));
        }
    }

    public class VideoUploadedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
