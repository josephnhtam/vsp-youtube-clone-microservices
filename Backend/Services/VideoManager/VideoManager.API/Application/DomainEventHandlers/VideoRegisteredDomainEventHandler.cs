using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.DomainEvents;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoRegisteredDomainEventHandler : IDomainEventHandler<VideoRegisteredDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoRegisteredDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoRegisteredDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            _transactionalEventsContext.AddOutboxMessage(
                new NotifyVideoRegisteredIntegrationEvent(
                    video.Id,
                    video.CreatorId));

            _transactionalEventsContext.AddOutboxMessage(
                new VideoInfoUpdatedIntegrationEvent(
                    video.Id,
                    video.Title,
                    video.Description,
                    video.Tags,
                    video.Thumbnail?.Url,
                    video.PreviewThumbnail?.Url,
                    video.Visibility,
                    video.AllowedToPublish,
                    video.InfoVersion));

            _transactionalEventsContext.AddOutboxMessage(
                new RequestVideoProcessingIntegrationEvent(
                    video.Id,
                    video.CreatorId,
                    video.OriginalVideoFileName!,
                    video.OriginalVideoUrl!));

            return Task.CompletedTask;
        }

    }
}
