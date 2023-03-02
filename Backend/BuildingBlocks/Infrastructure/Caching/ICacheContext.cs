namespace Infrastructure.Caching {
    public interface ICacheContext {
        void AddCache (string key, object value);
        void AddCache (string key, object value, DateTimeOffset expiration);
        void AddCache (string key, object value, TimeSpan expirationRelativeFromNow);
        void RemoveCache (string key);
        ValueTask<T?> GetCacheAsync<T> (string key, CancellationToken cancellationToken = default);
        ValueTask<object?> GetCacheAsync (string key, CancellationToken cancellationToken = default);
        ValueTask<Dictionary<string, T?>> GetCachesAsync<T> (IEnumerable<string> keys, CancellationToken cancellationToken = default);
        ValueTask<Dictionary<string, object?>> GetCachesAsync (IEnumerable<string> keys, CancellationToken cancellationToken = default);
        ValueTask CommitAsync (CancellationToken cancellationToken = default);
        void ClearLocalCaches ();
    }
}
