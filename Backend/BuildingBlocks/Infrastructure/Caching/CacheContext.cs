using Infrastructure.Caching.Layers;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Infrastructure.Caching {
    public class CacheContext : ICacheContext {

        protected static readonly ActivitySource _activitySource = new ActivitySource("CacheContext");

        private readonly IEnumerable<ICachingLayer> _layers;

        private ConcurrentDictionary<string, UncommittedCache> _uncommittedCaches;
        private ConcurrentBag<string> _uncommittedRemovals;
        private ConcurrentDictionary<string, object?> _localCaches;

        public CacheContext (IEnumerable<ICachingLayer> layers) {
            _layers = layers;

            _uncommittedCaches = new ConcurrentDictionary<string, UncommittedCache>();
            _uncommittedRemovals = new ConcurrentBag<string>();
            _localCaches = new ConcurrentDictionary<string, object?>();
        }

        public void AddCache (string key, object value) {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            _uncommittedCaches[key] = new UncommittedCache {
                Value = value,
                ExpirationDate = null
            };
        }

        public void AddCache (string key, object value, DateTimeOffset expiration) {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            _uncommittedCaches[key] = new UncommittedCache {
                Value = value,
                ExpirationDate = expiration
            };
        }

        public void AddCache (string key, object value, TimeSpan expiration) {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            _uncommittedCaches[key] = new UncommittedCache {
                Value = value,
                Expiration = expiration
            };
        }

        public void RemoveCache (string key) {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            _uncommittedCaches.TryRemove(key, out _);
            _uncommittedRemovals.Add(key);
        }

        public async ValueTask<object?> GetCacheAsync (string key, CancellationToken cancellationToken = default) {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            try {
                if (_uncommittedCaches.TryGetValue(key, out var cache)) {
                    return cache.Value;
                } else if (_localCaches.TryGetValue(key, out var cachedObject)) {
                    return cachedObject;
                }

                var result = await DoGetCacheAsync(key, cancellationToken);
                if (result != null) {
                    _localCaches[key] = result;
                }

                return result;
            } catch (Exception) {
                return null;
            }
        }

        public async ValueTask<T?> GetCacheAsync<T> (string key, CancellationToken cancellationToken = default) {
            return (T?)(await GetCacheAsync(key, cancellationToken));
        }

        public async ValueTask<Dictionary<string, object?>> GetCachesAsync (IEnumerable<string> _keys, CancellationToken cancellationToken = default) {
            _keys = _keys.Where(key => !string.IsNullOrEmpty(key));

            if (_keys.Count() == 0) return new Dictionary<string, object?>();

            try {
                List<string> keys = new List<string>(_keys);
                Dictionary<string, object?> results = new Dictionary<string, object?>();

                for (int i = 0; i < keys.Count; i++) {
                    var key = keys[i];

                    if (_uncommittedCaches.TryGetValue(key, out var cache)) {
                        results[key] = cache.Value;
                        keys.RemoveAt(i--);
                    } else if (_localCaches.TryGetValue(key, out var cachedObject)) {
                        results[key] = cachedObject;
                        keys.RemoveAt(i--);
                    }
                }

                var retrievedCaches = await DoGetCachesAsync(keys, cancellationToken);

                foreach (var (key, cache) in retrievedCaches) {
                    if (cache != null) {
                        _localCaches[key] = cache;
                        results[key] = cache;
                    }
                }

                return results;
            } catch (Exception) {
                return new Dictionary<string, object?>();
            }
        }

        public async ValueTask<Dictionary<string, T?>> GetCachesAsync<T> (IEnumerable<string> keys, CancellationToken cancellationToken = default) {
            var caches = await GetCachesAsync(keys, cancellationToken);
            var result = new Dictionary<string, T?>();

            foreach (var cache in caches) {
                try {
                    var value = (T?)cache.Value;
                    if (value != null) {
                        result[cache.Key] = value;
                    }
                } catch (Exception) { }
            }

            return result;
        }

        public async ValueTask CommitAsync (CancellationToken cancellationToken = default) {
            var uncommittedCaches =
                new Dictionary<string, UncommittedCache>(
                    Interlocked.Exchange(ref _uncommittedCaches, new ConcurrentDictionary<string, UncommittedCache>())
                );

            var uncommittedRemovals =
                Interlocked.Exchange(ref _uncommittedRemovals, new ConcurrentBag<string>())
                .Where(x => !uncommittedCaches.ContainsKey(x)).Distinct().ToList();

            if (uncommittedCaches.Count == 0 && uncommittedRemovals.Count == 0) {
                return;
            }

            foreach (var toRemove in uncommittedRemovals) {
                _localCaches.TryRemove(toRemove, out _);
            }

            foreach (var toAdd in uncommittedCaches) {
                _localCaches[toAdd.Key] = toAdd.Value.Value;
            }

            await DoCommitAsync(uncommittedCaches, uncommittedRemovals, cancellationToken);
        }

        public void ClearLocalCaches () {
            _localCaches.Clear();
        }

        private async ValueTask<object?> DoGetCacheAsync (string key, CancellationToken cancellationToken = default) {
            foreach (var layer in _layers) {
                var result = await layer.GetCacheAsync(key, cancellationToken);
                if (result != null) return result;
            }
            return null;
        }

        private async ValueTask<List<(string key, object? value)>> DoGetCachesAsync (List<string> keys, CancellationToken cancellationToken = default) {
            var results = new List<(string key, object? cache)>(keys.Count);

            foreach (var layer in _layers) {
                if (keys.Count == 0) break;

                var layerResults = (await layer.GetCachesAsync(keys, cancellationToken)).Where(x => x.value != null);

                foreach (var layerResult in layerResults) {
                    results.Add(layerResult);
                    keys.Remove(layerResult.key);
                }
            }

            return results;
        }

        private async ValueTask DoCommitAsync (Dictionary<string, UncommittedCache> uncommittedCaches, List<string> uncommittedRemovals, CancellationToken cancellationToken) {
            var tasks = new List<Task>(_layers.Count());

            foreach (var layer in _layers) {
                tasks.Add(layer.CommitAsync(uncommittedCaches, uncommittedRemovals, cancellationToken).AsTask());
            }

            await Task.WhenAll(tasks);
        }

    }
}
