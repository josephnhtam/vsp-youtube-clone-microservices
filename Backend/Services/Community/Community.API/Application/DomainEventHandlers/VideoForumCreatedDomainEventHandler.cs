using Community.Domain.DomainEvents;
using Domain.Events;

namespace Community.API.Application.DomainEventHandlers {
    public class VideoForumCreatedDomainEventHandler : IDomainEventHandler<VideoForumCreatedDomainEvent> {

        public Task Handle (VideoForumCreatedDomainEvent @event, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

    }
}
