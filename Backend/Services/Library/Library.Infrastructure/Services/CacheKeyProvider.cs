using Library.Infrastructure.Contracts;

namespace Library.Infrastructure.Services {
    public class CacheKeyProvider : ICacheKeyProvider {

        public string GetVideoCacheKey (Guid videoId) {
            return $"library-video-{videoId}";
        }

        public string GetUserProfileCacheKey (string userId) {
            return $"library-userProfile-{userId}";
        }

    }
}
