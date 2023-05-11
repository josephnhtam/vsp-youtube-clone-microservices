using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoProcessor.Application.IntegrationEvents;
using VideoProcessor.Domain.DomainEvents;

namespace VideoProcessor.Application.DomainEventHandlers {
    public class VideoProcessingStartedDomainEventHandler : IDomainEventHandler<VideoProcessingStartedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoProcessingStartedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoProcessingStartedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(new VideoProcessingStartedIntegrationEvent(@event.VideoId));
            return Task.CompletedTask;
        }
    }
}
