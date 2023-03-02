using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching.Layers {
    public class InMemoryCacheLayer : ICachingLayer {

        private readonly IMemoryCache _memoryCache;
        private readonly InMemoryCacheLayerConfiguration _config;

        public InMemoryCacheLayer (IMemoryCache memoryCache, IOptions<InMemoryCacheLayerConfiguration> config) {
            _memoryCache = memoryCache;
            _config = config.Value;
        }

        public ValueTask CommitAsync (IReadOnlyDictionary<string, UncommittedCache> uncommittedCaches, IReadOnlyList<string> uncommittedRemovals, CancellationToken cancellationToken) {
            foreach (var toRemove in uncommittedRemovals) {
                _memoryCache.Remove(toRemove);
            }

            foreach (var toAdd in uncommittedCaches) {
                var cacheOptions = new MemoryCacheEntryOptions {
                    Size = 1
                };

                TimeSpan? expiration = null;
                if (toAdd.Value.Expiration.HasValue) {
                    expiration = toAdd.Value.Expiration.Value * _config.ExpirationMultiplier;
                } else if (toAdd.Value.ExpirationDate.HasValue) {
                    expiration = (toAdd.Value.ExpirationDate.Value - DateTimeOffset.UtcNow) * _config.ExpirationMultiplier;
                }

                if (expiration.HasValue) {
                    if (expiration > _config.MaxExpiration) {
                        expiration = _config.MaxExpiration;
                    }
                    cacheOptions = cacheOptions.SetAbsoluteExpiration(expiration.Value);
                } else {
                    cacheOptions = cacheOptions.SetAbsoluteExpiration(_config.MaxExpiration);
                }

                _memoryCache.Set(toAdd.Key, toAdd.Value.Value, cacheOptions);
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask<object?> GetCacheAsync (string key, CancellationToken cancellationToken = default) {
            if (_memoryCache.TryGetValue(key, out var value)) {
                return ValueTask.FromResult(value);
            } else {
                return ValueTask.FromResult((object?)null);
            }
        }

        public ValueTask<List<(string key, object? value)>> GetCachesAsync (IReadOnlyList<string> keys, CancellationToken cancellationToken = default) {
            return ValueTask.FromResult(keys.Select(key => {
                if (_memoryCache.TryGetValue(key, out var value)) {
                    return (key, value);
                } else {
                    return (key, null);
                }
            }).ToList());
        }
    }
}
