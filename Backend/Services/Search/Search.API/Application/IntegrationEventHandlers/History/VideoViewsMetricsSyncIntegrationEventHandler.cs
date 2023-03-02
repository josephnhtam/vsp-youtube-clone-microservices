using EventBus;
using EventBus.RabbitMQ;
using MediatR;
using Search.API.Application.Commands;
using Search.API.Application.IntegrationEvents.History;

namespace Search.API.Application.IntegrationEventHandlers.History {
    public class VideoViewsMetricsSyncIntegrationEventHandler : IntegrationEventHandler<
        VideoViewsMetricsSyncIntegrationEvent,
        VideoViewsMetricsSyncIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoViewsMetricsSyncIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoViewsMetricsSyncIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UpdateVideoSearchInfoViewsMetricsCommand(
                integrationEvent.VideoId,
                integrationEvent.ViewsCount,
                integrationEvent.UpdateDate));
        }
    }

    public class VideoViewsMetricsSyncIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Search." + properties.QueueName;

            if (properties is RabbitMQIntegrationEventQueueProperties rmq) {
                rmq.BindingArguments = new Dictionary<string, object> {
                    { "x-match", "all" },
                    { "public", true }
                };
            }
        }
    }
}
