using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Storage.Shared.IntegrationEvents;
using VideoProcessor.Application.IntegrationEvents;
using VideoProcessor.Domain.DomainEvents;

namespace VideoProcessor.Application.DomainEventHandlers {
    public class VideoProcessingFailedDomainEventHandler : IDomainEventHandler<VideoProcessingFailedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoProcessingFailedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoProcessingFailedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(new VideoProcessingFailedIntegrationEvent(@event.VideoId));
            _transactionalEventsContext.AddOutboxMessage(new FilesCleanupIntegrationEvent(@event.VideoId));
            return Task.CompletedTask;
        }

    }
}
