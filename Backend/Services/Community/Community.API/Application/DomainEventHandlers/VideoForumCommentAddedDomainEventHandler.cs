using Community.API.Application.IntegrationEvents;
using Community.Domain.DomainEvents;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;

namespace Community.API.Application.DomainEventHandlers {
    public class VideoForumCommentAddedDomainEventHandler : IDomainEventHandler<VideoForumCommentAddedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoForumCommentAddedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoForumCommentAddedDomainEvent @event, CancellationToken cancellationToken) {
            var videoForum = @event.VideoForum;

            _transactionalEventsContext.AddOutboxMessage(
                new VideoCommentsMetricsSyncIntegrationEvent(
                    videoForum.VideoId, videoForum.VideoCommentsCount, DateTimeOffset.UtcNow));

            return Task.CompletedTask;
        }

    }
}
