using Domain.Events;
using VideoStore.Domain.DomainEvents;

namespace VideoStore.API.Application.DomainEventHandlers {
    public class VideoCreatedDomainEventHandler : IDomainEventHandler<VideoCreatedDomainEvent> {

        public Task Handle (VideoCreatedDomainEvent @event, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

    }
}
