using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.DomainEvents;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoProcessingFailedDomainEventHandler : IDomainEventHandler<VideoProcessingFailedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoProcessingFailedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoProcessingFailedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new NotifyVideoProcessingFailedIntegrationEvent(
                    @event.Video.Id,
                    @event.Video.CreatorId));

            _transactionalEventsContext.AddOutboxMessage(
                new UnregisterVideoIntegrationEvent(
                    @event.Video.Id));

            return Task.CompletedTask;
        }

    }
}
