using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.DomainEvents;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoUnregisteredDomainEventHandler : IDomainEventHandler<VideoUnregisteredDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoUnregisteredDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoUnregisteredDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            if (video.ProcessingStatus != VideoProcessingStatus.WaitingForUserUpload &&
                video.ProcessingStatus != VideoProcessingStatus.VideoProcessingFailed) {
                _transactionalEventsContext.AddOutboxMessage(
                    new UnregisterVideoIntegrationEvent(
                        @event.Video.Id));
            }

            return Task.CompletedTask;
        }

    }
}
