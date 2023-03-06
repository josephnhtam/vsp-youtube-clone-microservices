using Domain.Events;
using Library.Domain.Contracts;
using Library.Domain.DomainEvents.Videos;
using Library.Infrastructure.Contracts;

namespace Library.API.Application.DomainEventHandlers {
    public class VideoUnregisteredDomainEventHandler : IDomainEventHandler<VideoUnregisteredDomainEvent> {

        private readonly IVideoRepository _videoRepository;
        private readonly ICachedVideoRepository _cachedVideoRepository;
        private readonly ILogger<VideoUnregisteredDomainEventHandler> _logger;

        public VideoUnregisteredDomainEventHandler (IVideoRepository videoRepository, ICachedVideoRepository cachedVideoRepository, ILogger<VideoUnregisteredDomainEventHandler> logger) {
            _videoRepository = videoRepository;
            _cachedVideoRepository = cachedVideoRepository;
            _logger = logger;
        }

        public async Task Handle (VideoUnregisteredDomainEvent @event, CancellationToken cancellationToken) {
            await _videoRepository.RemoveVideoAsync(@event.VideoId, cancellationToken);

            try {
                await _cachedVideoRepository.RemoveVideoCachesAsync(new Guid[] { @event.VideoId });
                _logger.LogInformation("Removed caching for video ({VideoId})", @event.VideoId);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to remove caching for video ({VideoId})", @event.VideoId);
            }
        }

    }
}
