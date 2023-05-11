using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoProcessor.Application.DtoModels;
using VideoProcessor.Application.IntegrationEvents;
using VideoProcessor.Domain.DomainEvents;

namespace VideoProcessor.Application.DomainEventHandlers {
    public class VideoProcessingVideoAddedDomainEventHandler : IDomainEventHandler<VideoProcessingVideoAddedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public VideoProcessingVideoAddedDomainEventHandler (ITransactionalEventsContext transactionalEventsContext, IMapper mapper) {
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public Task Handle (VideoProcessingVideoAddedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new VideoProcessingVideoAddedIntegrationEvent(
                    @event.VideoId,
                   _mapper.Map<ProcessedVideoDto>(@event.Video))
                );

            return Task.CompletedTask;
        }

    }
}
