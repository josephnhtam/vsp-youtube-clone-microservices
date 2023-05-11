using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.DomainEvents;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class VideoProcessedDomainEventHandler : IDomainEventHandler<VideoProcessedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public VideoProcessedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext,
            IMapper mapper) {
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public Task Handle (VideoProcessedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new NotifyVideoProcessedIntegrationEvent(
                    _mapper.Map<VideoDto>(@event.Video, options => options.Items["resolveUrl"] = true)));

            _transactionalEventsContext.AddOutboxMessage(
                new SetVideoReadyIntegrationEvent(
                    @event.Video.Id,
                    @event.Video.Title,
                    @event.Video.Description,
                    @event.Video.Tags,
                    @event.Video.Thumbnail?.Url,
                    @event.Video.PreviewThumbnail?.Url,
                    @event.Video.Visibility,
                    @event.Video.AllowedToPublish,
                    _mapper.Map<List<ProcessedVideoDto>>(@event.Video.Videos),
                    @event.Video.InfoVersion));

            return Task.CompletedTask;
        }

    }
}
