using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoProcessor.Application.DtoModels;
using VideoProcessor.Application.IntegrationEvents;
using VideoProcessor.Domain.DomainEvents;

namespace VideoProcessor.Application.DomainEventHandlers {
    public class VideoProcessingThumbnailsAddedDomainEventHandler : IDomainEventHandler<VideoProcessingThumbnailsAddedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public VideoProcessingThumbnailsAddedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext, IMapper mapper) {
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public Task Handle (VideoProcessingThumbnailsAddedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new VideoProcessingThumbnailsAddedIntegrationEvent(
                    @event.VideoId,
                    _mapper.Map<List<VideoThumbnailDto>>(@event.Thumbnails),
                    _mapper.Map<VideoPreviewThumbnailDto>(@event.PreviewThumbnail))
                );

            return Task.CompletedTask;
        }

    }
}
