using Domain.Events;
using Library.Domain.DomainEvents.Videos;

namespace Library.API.Application.DomainEventHandlers {
    public class VideoCreatedDomainEventHandler : IDomainEventHandler<VideoCreatedDomainEvent> {

        public Task Handle (VideoCreatedDomainEvent @event, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

    }
}
