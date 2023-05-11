using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.DomainEvents;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoBeingProcessedDomainEventHandler : IDomainEventHandler<VideoBeingProcessedDomainEvent> {

        private readonly IVideoRepository _repository;
        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoBeingProcessedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoBeingProcessedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new NotifyVideoBeingProcessedIntegrationEvent(
                    @event.Video.Id,
                    @event.Video.CreatorId));

            return Task.CompletedTask;
        }

    }
}
