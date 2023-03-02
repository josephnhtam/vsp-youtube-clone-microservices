using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.Community;

namespace VideoManager.API.Application.IntegrationEventHandlers.History {
    public class VideoCommentsMetricsSyncIntegrationEventHandler : IntegrationEventHandler<
        VideoCommentsMetricsSyncIntegrationEvent,
        VideoCommentsMetricsSyncIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoCommentsMetricsSyncIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoCommentsMetricsSyncIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UpdateVideoCommentsMetricsCommand(
                integrationEvent.VideoId,
                integrationEvent.CommentsCount,
                integrationEvent.UpdateDate));
        }
    }

    public class VideoCommentsMetricsSyncIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
