using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoStore.API.Application.IntegrationEvents;
using VideoStore.Domain.DomainEvents;

namespace VideoStore.API.Application.DomainEventHandlers {
    public class VideoPublishedWithPublicVisibilityDomainEventHandler : IDomainEventHandler<VideoPublishedWithPublicVisibilityDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoPublishedWithPublicVisibilityDomainEventHandler (ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoPublishedWithPublicVisibilityDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            _transactionalEventsContext.AddOutboxMessage(
                new VideoPublishedWithPublicVisibilityIntegrationEvent(
                   video.Id,
                   video.CreatorId,
                   video.Title,
                   video.Description,
                   video.Tags,
                   video.ThumbnailUrl,
                   video.PreviewThumbnailUrl,
                   video.Videos.FirstOrDefault()?.LengthSeconds,
                   video.StatusUpdateDate,
                   video.Version
                ));

            return Task.CompletedTask;
        }

    }
}
