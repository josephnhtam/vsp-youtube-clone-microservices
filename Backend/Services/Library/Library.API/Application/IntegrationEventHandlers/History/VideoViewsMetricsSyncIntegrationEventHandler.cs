using EventBus;
using Library.API.Application.Commands;
using Library.API.Application.IntegrationEvents.History;
using MediatR;

namespace Library.API.Application.IntegrationEventHandlers.History {
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
                    integrationEvent.UpdateDate
                ));
        }

    }

    public class VideoViewsMetricsSyncIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Library." + properties.QueueName;
        }
    }
}
