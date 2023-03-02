using History.Infrastructure.Contracts;

namespace History.Infrastructure.Services {
    public class CacheKeyProvider : ICacheKeyProvider {

        public string GetVideoCacheKey (Guid videoId) {
            return $"history-video-{videoId}";
        }

        public string GetUserProfileCacheKey (string userId) {
            return $"history-userProfile-{userId}";
        }

    }
}
