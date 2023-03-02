namespace Infrastructure.Caching.Layers {
    public interface ICachingLayer {
        ValueTask CommitAsync (IReadOnlyDictionary<string, UncommittedCache> uncommittedCaches, IReadOnlyList<string> uncommittedRemovals, CancellationToken cancellationToken);
        ValueTask<object?> GetCacheAsync (string key, CancellationToken cancellationToken = default);
        ValueTask<List<(string key, object? value)>> GetCachesAsync (IReadOnlyList<string> keys, CancellationToken cancellationToken = default);
    }

    public struct UncommittedCache {
        public object Value;
        public DateTimeOffset? ExpirationDate;
        public TimeSpan? Expiration;
    }
}
