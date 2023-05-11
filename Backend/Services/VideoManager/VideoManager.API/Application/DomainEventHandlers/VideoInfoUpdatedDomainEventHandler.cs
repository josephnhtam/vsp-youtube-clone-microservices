using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.DomainEvents;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoInfoUpdatedDomainEventHandler : IDomainEventHandler<VideoInfoUpdatedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoInfoUpdatedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (VideoInfoUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            if (video.Status != VideoStatus.Registered ||
                video.ProcessingStatus == VideoProcessingStatus.VideoProcessingFailed) {
                return Task.CompletedTask;
            }

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

            return Task.CompletedTask;
        }

    }
}
