using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Storage.Shared.IntegrationEvents;
using VideoProcessor.Application.DtoModels;
using VideoProcessor.Application.IntegrationEvents;
using VideoProcessor.Domain.DomainEvents;

namespace VideoProcessor.Application.DomainEventHandlers {
    public class VideoProcessingCompleteDomainEventHandler : IDomainEventHandler<VideoProcessingCompleteDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public VideoProcessingCompleteDomainEventHandler (ITransactionalEventsContext transactionalEventsContext, IMapper mapper) {
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public Task Handle (VideoProcessingCompleteDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new VideoProcessingCompleteIntegrationEvent(
                    @event.VideoId,
                    _mapper.Map<List<VideoThumbnailDto>>(@event.Thumbnails),
                    _mapper.Map<VideoPreviewThumbnailDto>(@event.PreviewThumbnail),
                    _mapper.Map<List<ProcessedVideoDto>>(@event.Videos))
                );

            var fileIds = @event.Thumbnails.Select(x => x.ImageFileId)
                .Union(@event.Videos.Select(x => x.VideoFileId)).ToList();

            if (@event.PreviewThumbnail != null) {
                fileIds.Add(@event.PreviewThumbnail.ImageFileId);
            }

            _transactionalEventsContext.AddOutboxMessage(
                new FilesCleanupIntegrationEvent(
                    @event.VideoId,
                    fileIds
                )
            );

            return Task.CompletedTask;
        }

    }
}
