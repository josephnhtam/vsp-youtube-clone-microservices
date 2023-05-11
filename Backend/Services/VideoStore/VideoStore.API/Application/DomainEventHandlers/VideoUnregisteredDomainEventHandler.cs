using Domain.Events;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Storage.Shared.IntegrationEvents;
using VideoStore.Domain.Contracts;
using VideoStore.Domain.DomainEvents;

namespace VideoStore.API.Application.DomainEventHandlers {
    public class VideoUnregisteredDomainEventHandler : IDomainEventHandler<VideoUnregisteredDomainEvent> {

        private readonly IVideoRepository _videoRepository;
        private readonly ITransactionalEventsContext _transactionalEventsContext;

        public VideoUnregisteredDomainEventHandler (IVideoRepository videoRepository, ITransactionalEventsContext transactionalEventsContext) {
            _videoRepository = videoRepository;
            _transactionalEventsContext = transactionalEventsContext;
        }

        public async Task Handle (VideoUnregisteredDomainEvent @event, CancellationToken cancellationToken) {
            var video = @event.Video;

            _transactionalEventsContext.AddOutboxMessage(new FilesCleanupIntegrationEvent(video.Id));

            await _videoRepository.RemoveVideoAsync(video, cancellationToken);
        }

    }
}
