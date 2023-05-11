using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.DomainEvents;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoThumbnailsUpdatedDomainEventHandler : IDomainEventHandler<VideoThumbnailsUpdatedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public VideoThumbnailsUpdatedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext,
            IMapper mapper) {
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public Task Handle (VideoThumbnailsUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            _transactionalEventsContext.AddOutboxMessage(
                new NotifyVideoThumbnailsAddedIntegrationEvent(
                    video.Id,
                    video.CreatorId,
                    _mapper.Map<List<VideoThumbnailDto>>(video.Thumbnails, options => options.Items["resolveUrl"] = true)));

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
