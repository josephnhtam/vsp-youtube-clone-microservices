using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SharedKernel.Utilities;
using StackExchange.Redis;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Infrastructure.Caching.Layers {
    public class RedisCacheLayer : ICachingLayer {

        protected static readonly ActivitySource _activitySource = new ActivitySource("CacheContext");

        private readonly IDatabase _database;
        private readonly RedisCacheLayerConfiguration _config;
        private readonly ILogger<RedisCacheLayer> _logger;

        private static JsonSerializer _jsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings {
            ContractResolver = new CacheContractResolver()
        });

        public RedisCacheLayer (IConnectionMultiplexer redisConn, IOptions<RedisCacheLayerConfiguration> config, ILogger<RedisCacheLayer> logger) {
            _database = redisConn.GetDatabase();
            _config = config.Value;
            _logger = logger;
        }

        public async ValueTask CommitAsync (IReadOnlyDictionary<string, UncommittedCache> uncommittedCaches, IReadOnlyList<string> uncommittedRemovals, CancellationToken cancellationToken) {
            using var activity = _activitySource.StartActivity("Commit caches to redis database");
            activity?.SetTag("Keys", string.Join(", ", uncommittedCaches.Keys));

            try {
                if (uncommittedCaches.Count + uncommittedRemovals.Count > 1) {
                    await DoCommitInBatch(uncommittedCaches, uncommittedRemovals, cancellationToken);
                } else {
                    await DoCommitWithoutBatch(uncommittedCaches, uncommittedRemovals, cancellationToken);
                }
            } catch (Exception ex) {
                activity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                _logger.LogError(ex, "Failed to commit caches to redis database");
                throw;
            }
        }

        private async Task DoCommitWithoutBatch (IReadOnlyDictionary<string, UncommittedCache> uncommittedCaches, IReadOnlyList<string> uncommittedRemovals, CancellationToken cancellationToken) {
            foreach (var toRemove in uncommittedRemovals) {
                await _database.KeyDeleteAsync(toRemove);
            }

            foreach (var (key, toAdd) in uncommittedCaches) {
                if (toAdd.Value != null) {
                    var data = Serialize(toAdd.Value);

                    TimeSpan? expiration = null;
                    if (toAdd.Expiration.HasValue) {
                        expiration = toAdd.Expiration.Value * _config.ExpirationMultiplier;
                    } else if (toAdd.ExpirationDate.HasValue) {
                        expiration = (toAdd.ExpirationDate.Value - DateTimeOffset.UtcNow) * _config.ExpirationMultiplier;
                    }

                    if (expiration.HasValue) {
                        if (expiration > _config.MaxExpiration) {
                            expiration = _config.MaxExpiration;
                        }

                        await _database.StringSetAsync(key, data, expiration.Value);
                    } else {
                        await _database.StringSetAsync(key, data, _config.MaxExpiration);
                    }
                }
            }
        }

        private async Task DoCommitInBatch (IReadOnlyDictionary<string, UncommittedCache> uncommittedCaches, IReadOnlyList<string> uncommittedRemovals, CancellationToken cancellationToken) {
            var tasks = new List<Task>();

            var batch = _database.CreateBatch();

            foreach (var toRemove in uncommittedRemovals) {
                tasks.Add(batch.KeyDeleteAsync(toRemove));
            }

            foreach (var (key, toAdd) in uncommittedCaches) {
                if (toAdd.Value != null) {
                    var data = Serialize(toAdd.Value);

                    TimeSpan? expiration = null;
                    if (toAdd.Expiration.HasValue) {
                        expiration = toAdd.Expiration.Value * _config.ExpirationMultiplier;
                    } else if (toAdd.ExpirationDate.HasValue) {
                        expiration = (toAdd.ExpirationDate.Value - DateTimeOffset.UtcNow) * _config.ExpirationMultiplier;
                    }

                    if (expiration.HasValue) {
                        if (expiration > _config.MaxExpiration) {
                            expiration = _config.MaxExpiration;
                        }

                        tasks.Add(_database.StringSetAsync(key, data, expiration.Value));
                    } else {
                        tasks.Add(_database.StringSetAsync(key, data, _config.MaxExpiration));
                    }
                }
            }

            batch.Execute();

            await Task.WhenAll(tasks).WithCancellation(cancellationToken);
        }

        public async ValueTask<object?> GetCacheAsync (string key, CancellationToken cancellationToken = default) {
            using var activity = _activitySource.StartActivity("Get cache from redis database");
            activity?.SetTag("Key", key);

            try {
                var data = await _database.StringGetAsync(key);

                if (data.HasValue) {
                    return Deserialize(data.ToString());
                } else {
                    return null;
                }
            } catch (Exception ex) {
                activity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                _logger.LogError(ex, "Failed to get cache ({Key}) from redis database", key);
                throw;
            }
        }

        public async ValueTask<List<(string key, object? value)>> GetCachesAsync (IReadOnlyList<string> keys, CancellationToken cancellationToken = default) {
            using var activity = _activitySource.StartActivity("Get caches from redis database");
            activity?.SetTag("Keys", string.Join(", ", keys));

            try {
                var datas = await _database.StringGetAsync(keys.Select(x => new RedisKey(x)).ToArray());

                return keys.Select((key, index) => {
                    var data = datas[index];
                    return (key, data.HasValue ? Deserialize(data.ToString()) : null);
                }).ToList();
            } catch (Exception ex) {
                activity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                _logger.LogError(ex, "Failed to get caches ({Keys}) from redis database", keys);
                throw;
            }
        }

        private string Serialize (object value) {
            var type = value.GetType();
            var typeName = type.FullName!;
            var json = SerializeObject(value, type);

            return $"{typeName}|{json}";
        }

        private object? Deserialize (string? data) {
            if (string.IsNullOrEmpty(data)) return null;

            var separatorIndex = data.IndexOf('|');
            if (separatorIndex == -1) return null;

            var typeName = data.Substring(0, separatorIndex);
            var json = data.Substring(separatorIndex + 1);

            var type = TypeCache.GetType(typeName);

            if (type == null) {
                _logger.LogError("The type is not found ({TypeName})", typeName);
                return null;
            }

            try {
                return DeserializeObject(json, type);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to deserialize cache ({Json})", json);
                throw;
            }
        }

        private static string SerializeObject (object? value, Type? type) {
            StringBuilder sb = new StringBuilder(256);
            using (StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture)) {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(sw)) {
                    _jsonSerializer.Serialize(jsonWriter, value, type);
                    return sw.ToString();
                }
            }
        }

        private static object? DeserializeObject (string value, Type? type) {
            using (JsonTextReader reader = new JsonTextReader(new StringReader(value))) {
                return _jsonSerializer.Deserialize(reader, type);
            }
        }

    }
}
