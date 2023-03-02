using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.History;

namespace VideoManager.API.Application.IntegrationEventHandlers.History {
    public class VideoViewsMetricsSyncIntegrationEventHandler : IntegrationEventHandler<
        VideoViewsMetricsSyncIntegrationEvent,
        VideoViewsMetricsSyncIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoViewsMetricsSyncIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoViewsMetricsSyncIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UpdateVideoViewsMetricsCommand(
                integrationEvent.VideoId,
                integrationEvent.ViewsCount,
                integrationEvent.UpdateDate));
        }
    }

    public class VideoViewsMetricsSyncIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
