using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoStore.API.Application.IntegrationEvents;
using VideoStore.Domain.DomainEvents;

namespace VideoStore.API.Application.DomainEventHandlers {
    public class VideoPublishedDomainEventHandler : IDomainEventHandler<VideoPublishedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoPublishedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoPublishedDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            _transactionalEventsContext.AddOutboxMessage(
                new VideoPublishedIntegrationEvent(
                   video.Id,
                   video.StatusUpdateDate!.Value,
                   video.Version
                ));

            return Task.CompletedTask;
        }

    }
}
