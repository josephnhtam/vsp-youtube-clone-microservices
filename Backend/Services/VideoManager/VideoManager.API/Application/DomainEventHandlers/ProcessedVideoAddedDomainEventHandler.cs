using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Application.IntegrationEvents;
using VideoManager.Domain.DomainEvents;

namespace VideoManager.API.Application.DomainEventHandlers {
    public class ProcessedVideoAddedDomainEventHandler : IDomainEventHandler<ProcessedVideoAddedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public ProcessedVideoAddedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext,
            IMapper mapper) {
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public Task Handle (ProcessedVideoAddedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new NotifyProcessedVideoAddedIntegrationEvent(
                    @event.Video.Id,
                    @event.Video.CreatorId,
                    _mapper.Map<ProcessedVideoDto>(@event.ProcessedVideo, options => options.Items["resolveUrl"] = true)));

            _transactionalEventsContext.AddOutboxMessage(
                new UpdateStoreVideoResourcesIntegrationEvent(
                    @event.Video.Id,
                    _mapper.Map<List<ProcessedVideoDto>>(@event.Video.Videos),
                    true));

            return Task.CompletedTask;
        }

    }
}
