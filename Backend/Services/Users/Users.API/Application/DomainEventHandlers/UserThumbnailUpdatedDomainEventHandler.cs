using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Storage.Shared.IntegrationEvents;
using Users.Domain.DomainEvents;

namespace Users.API.Application.DomainEventHandlers {
    public class UserThumbnailUpdatedDomainEventHandler : IDomainEventHandler<UserProfileThumbnailUpdatedDomainEvent> {

        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public UserThumbnailUpdatedDomainEventHandler (
            ITransactionalEventsContext transactionalEventsContext) {
            _transactionalEventsContext = transactionalEventsContext;
        }

        public Task Handle (UserProfileThumbnailUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            if (@event.OldThumbnail != null && @event.OldThumbnail.ImageFileId != @event.UserProfile.Thumbnail?.ImageFileId) {
                _transactionalEventsContext.AddOutboxMessage(
                    new RemoveFileIntegrationEvent(new List<Guid>() { @event.OldThumbnail.ImageFileId }, TimeSpan.FromHours(24)));
            }

            if (@event.UserProfile.Thumbnail != null) {
                _transactionalEventsContext.AddOutboxMessage(
                    new SetFileInUseIntegrationEvent(new List<Guid>() { @event.UserProfile.Thumbnail.ImageFileId }));
            }

            return Task.CompletedTask;
        }
    }
}
