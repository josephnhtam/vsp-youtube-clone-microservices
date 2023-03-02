using Infrastructure.Caching;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Library.Infrastructure.Configurations;
using Library.Infrastructure.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Library.Infrastructure.Repositories {
    public class CachedVideoRepository : ICachedVideoRepository {

        private readonly IVideoRepository _videoRepository;
        private readonly ICacheContext _cacheContext;
        private readonly ICacheKeyProvider _cacheKeyProvider;
        private readonly CachingConfigurations _cachingConfigurations;
        private readonly ILogger<CachedVideoRepository> _logger;

        public CachedVideoRepository (IVideoRepository videoRepository,
            ICacheContext cacheContext,
            ICacheKeyProvider cacheKeyProvider,
            IOptions<CachingConfigurations> cachingConfigurations,
            ILogger<CachedVideoRepository> logger) {
            _videoRepository = videoRepository;
            _cacheContext = cacheContext;
            _cacheKeyProvider = cacheKeyProvider;
            _cachingConfigurations = cachingConfigurations.Value;
            _logger = logger;
        }

        public async Task<Video?> GetVideoByIdAsync (Guid id, CancellationToken cancellationToken = default) {
            try {
                Video? cachedVideo;

                cachedVideo =
                    await _cacheContext.GetCacheAsync<Video>(_cacheKeyProvider.GetVideoCacheKey(id), cancellationToken);

                if (cachedVideo != null) {
                    return cachedVideo;
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to retrieve video cache ({VideoId})", id);
            }

            var video = await _videoRepository.GetVideoByIdAsync(id, false, cancellationToken);

            if (video != null) {
                await CacheVideosAsync(new[] { video }, cancellationToken);
            }

            return video;
        }

        public async Task<List<Video>> GetVideosAsync (IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
            List<Video> cachedVideos;

            try {
                cachedVideos =
                    (await _cacheContext
                    .GetCachesAsync<Video>(ids.Select(_cacheKeyProvider.GetVideoCacheKey), cancellationToken))
                    .Select(x => x.Value).ToList()!;

            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to retrieve video caches");
                cachedVideos = new List<Video>();
            }

            var uncachedIds = ids.Where(id => !cachedVideos.Any(v => v.Id == id));

            if (uncachedIds.Count() > 0) {
                var videos = await _videoRepository.GetVideosAsync(uncachedIds, cancellationToken);
                await CacheVideosAsync(videos, cancellationToken);

                return cachedVideos.Union(videos).ToList();
            }

            return cachedVideos.ToList();
        }

        public async Task<List<Video>> GetAvailableVideosAsync (IEnumerable<Guid> ids, string? userId, CancellationToken cancellationToken = default) {
            var videos = await GetVideosAsync(ids, cancellationToken);

            if (!string.IsNullOrEmpty(userId)) {
                return videos.Where(v => v.Visibility != VideoVisibility.Private || v.CreatorId == userId).ToList();
            } else {
                return videos.Where(v => v.Visibility != VideoVisibility.Private).ToList();
            }
        }

        public async Task CacheVideosAsync (IEnumerable<Video> videos, CancellationToken cancellationToken = default) {
            try {
                var cacheDuration = TimeSpan.FromSeconds(_cachingConfigurations.VideoCacheDurationInSeconds);

                foreach (var video in videos) {
                    _cacheContext.AddCache(_cacheKeyProvider.GetVideoCacheKey(video.Id), video, cacheDuration);
                }

                await _cacheContext.CommitAsync(cancellationToken);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to add video caches");
            }
        }

        public async Task RemoveVideoCachesAsync (IEnumerable<Guid> videoIds, CancellationToken cancellationToken = default) {
            try {
                foreach (var videoId in videoIds) {
                    _cacheContext.RemoveCache(_cacheKeyProvider.GetVideoCacheKey(videoId));
                }

                await _cacheContext.CommitAsync(cancellationToken);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to remove video caches");
            }
        }

    }
}
