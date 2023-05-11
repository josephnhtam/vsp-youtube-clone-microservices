using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoStore.API.Application.IntegrationEvents;
using VideoStore.Domain.DomainEvents;

namespace VideoStore.API.Application.DomainEventHandlers {
    public class VideoUnpublishedDomainEventHandler : IDomainEventHandler<VideoUnpublishedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoUnpublishedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoUnpublishedDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            _transactionalEventsContext.AddOutboxMessage(
                new VideoUnpublishedIntegrationEvent(
                   video.Id,
                   video.StatusUpdateDate!.Value,
                   video.Version
                ));

            return Task.CompletedTask;
        }

    }
}
