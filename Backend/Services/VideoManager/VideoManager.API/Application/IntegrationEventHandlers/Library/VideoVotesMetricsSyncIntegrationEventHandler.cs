using EventBus;
using MediatR;
using VideoManager.API.Application.Commands;
using VideoManager.API.Application.IntegrationEvents.Library;

namespace VideoManager.API.Application.IntegrationEventHandlers.Library {
    public class VideoVotesMetricsSyncIntegrationEventHandler : IntegrationEventHandler<
        VideoVotesMetricsSyncIntegrationEvent,
        VideoVotesMetricsSyncIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public VideoVotesMetricsSyncIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (VideoVotesMetricsSyncIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new UpdateVideoVotesMetricsCommand(
                integrationEvent.VideoId,
                integrationEvent.LikesCount,
                integrationEvent.DislikesCount,
                integrationEvent.UpdateDate));
        }
    }

    public class VideoVotesMetricsSyncIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoManager." + properties.QueueName;
        }
    }
}
