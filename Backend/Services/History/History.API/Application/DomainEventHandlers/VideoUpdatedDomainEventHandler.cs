using Domain.Events;
using History.Domain.DomainEvents.Videos;
using History.Infrastructure.Contracts;

namespace History.API.Application.DomainEventHandlers {
    public class VideoUpdatedDomainEventHandler : IDomainEventHandler<VideoUpdatedDomainEvent> {

        private readonly ICachedVideoRepository _cachedVideoRepository;
        private readonly ILogger<VideoUpdatedDomainEventHandler> _logger;

        public VideoUpdatedDomainEventHandler (ICachedVideoRepository cachedVideoRepository, ILogger<VideoUpdatedDomainEventHandler> logger) {
            _cachedVideoRepository = cachedVideoRepository;
            _logger = logger;
        }

        public async Task Handle (VideoUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            try {
                await _cachedVideoRepository.RemoveVideoCachesAsync(new Guid[] { @event.VideoId });
                _logger.LogInformation("Removed caching for video ({VideoId})", @event.VideoId);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to remove caching for video ({VideoId})", @event.VideoId);
            }
        }

    }
}
