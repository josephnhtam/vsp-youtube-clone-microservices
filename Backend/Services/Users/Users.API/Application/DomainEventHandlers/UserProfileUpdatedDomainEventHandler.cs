using AutoMapper;
using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Users.API.Application.DtoModels;
using Users.API.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.API.Application.DomainEventHandlers {
    public class UserProfileUpdatedDomainEventHandler : IDomainEventHandler<UserProfileUpdatedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IMapper _mapper;

        public UserProfileUpdatedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext,
            IMapper mapper) {
            _transactionalEventsContext = transactionalEventsContext;
            _mapper = mapper;
        }

        public Task Handle (UserProfileUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            _transactionalEventsContext.AddOutboxMessage(
                new UserProfileCreatedOrUpdatedIntegrationEvent(_mapper.Map<InternalUserProfileDto>(@event.UserProfile)));
            return Task.CompletedTask;
        }
    }
}
