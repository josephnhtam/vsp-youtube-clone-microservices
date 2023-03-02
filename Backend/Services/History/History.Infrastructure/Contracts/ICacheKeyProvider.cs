namespace History.Infrastructure.Contracts {
    public interface ICacheKeyProvider {
        string GetVideoCacheKey (Guid videoId);
        string GetUserProfileCacheKey (string userId);
    }
}
