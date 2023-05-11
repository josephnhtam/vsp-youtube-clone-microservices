using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoStore.API.Application.IntegrationEvents;
using VideoStore.Domain.DomainEvents;

namespace VideoStore.API.Application.DomainEventHandlers {
    public class VideoUpdatedDomainEventHandler : IDomainEventHandler<VideoUpdatedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoUpdatedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            _transactionalEventsContext.AddOutboxMessage(
                new VideoUpdatedIntegrationEvent(
                   video.Id,
                   video.Title,
                   video.Description,
                   video.Tags,
                   video.ThumbnailUrl,
                   video.PreviewThumbnailUrl,
                   video.Videos.FirstOrDefault()?.LengthSeconds,
                   video.Visibility,
                   video.Status,
                   video.StatusUpdateDate,
                   video.Version
                ));

            return Task.CompletedTask;
        }

    }
}
