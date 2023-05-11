using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Storage.Shared.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.API.Application.DomainEventHandlers {
    public class UserBannerUpdatedDomainEventHandler : IDomainEventHandler<UserChannelBannerUpdatedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public UserBannerUpdatedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (UserChannelBannerUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            if (@event.OldBanner != null && @event.OldBanner.ImageFileId != @event.UserChannel.Banner?.ImageFileId) {
                _transactionalEventsContext.AddOutboxMessage(
                    new RemoveFileIntegrationEvent(new List<Guid>() { @event.OldBanner.ImageFileId }, TimeSpan.FromHours(24)));
            }

            if (@event.UserChannel.Banner != null) {
                _transactionalEventsContext.AddOutboxMessage(
                    new SetFileInUseIntegrationEvent(new List<Guid>() { @event.UserChannel.Banner.ImageFileId }));
            }

            return Task.CompletedTask;
        }
    }
}
