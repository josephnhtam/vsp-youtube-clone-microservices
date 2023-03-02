using EventBus;
using EventBus.RabbitMQ;
using MediatR;
using Search.API.Application.Commands;
using Search.API.Application.IntegrationEvents.Library;

namespace Search.API.Application.IntegrationEventHandlers.Library {
    public class VideoVotesMetricsSyncIntegrationEventHandler : IntegrationEventHandler<
        VideoVotesMetricsSyncIntegrationEvent,
        VideoVotesMetricsSyncIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoVotesMetricsSyncIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoVotesMetricsSyncIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UpdateVideoSearchInfoVotesMetricsCommand(
                integrationEvent.VideoId,
                integrationEvent.LikesCount,
                integrationEvent.DislikesCount,
                integrationEvent.UpdateDate));
        }
    }

    public class VideoVotesMetricsSyncIntegrationEventQueue : IntegrationEventQueue {
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
