using Application.Handlers;
using Community.Domain.DomainEvents;

namespace Community.API.Application.DomainEventHandlers {
    public class VideoForumCreatedDomainEventHandler : IDomainEventHandler<VideoForumCreatedDomainEvent> {

        public Task Handle (VideoForumCreatedDomainEvent @event, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

    }
}
