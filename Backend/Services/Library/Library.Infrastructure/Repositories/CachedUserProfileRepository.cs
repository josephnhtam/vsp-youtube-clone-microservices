using Infrastructure.Caching;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Library.Infrastructure.Configurations;
using Library.Infrastructure.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Library.Infrastructure.Repositories {
    public class CachedUserProfileRepository : ICachedUserProfileRepository {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ICacheContext _cacheContext;
        private readonly ICacheKeyProvider _cacheKeyProvider;
        private readonly CachingConfigurations _cachingConfigurations;
        private readonly ILogger<CachedUserProfileRepository> _logger;

        public CachedUserProfileRepository (
            IUserProfileRepository userProfileRepository,
            ICacheContext CacheContext,
            ICacheKeyProvider cacheKeyProvider,
            IOptions<CachingConfigurations> cachingConfigurations,
            ILogger<CachedUserProfileRepository> logger) {
            _userProfileRepository = userProfileRepository;
            _cacheContext = CacheContext;
            _cacheKeyProvider = cacheKeyProvider;
            _cachingConfigurations = cachingConfigurations.Value;
            _logger = logger;
        }

        public async Task<UserProfile?> GetUserProfileAsync (string userId, CancellationToken cancellationToken = default) {
            try {
                UserProfile? cachedUserProfile =
                    await _cacheContext.GetCacheAsync<UserProfile>(_cacheKeyProvider.GetUserProfileCacheKey(userId), cancellationToken);

                if (cachedUserProfile != null) {
                    return cachedUserProfile;
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to retrieve userProfile cache ({UserId})", userId);
            }

            var userProfile = await _userProfileRepository.GetUserProfileAsync(userId, false, cancellationToken);

            if (userProfile != null) {
                await CacheUserProfilesAsync(new[] { userProfile }, cancellationToken);
            }

            return userProfile;
        }

        public async Task<List<UserProfile>> GetUserProfilesAsync (IEnumerable<string> userIds, CancellationToken cancellationToken = default) {
            List<UserProfile> cachedUserProfiles;

            try {
                cachedUserProfiles =
                    (await _cacheContext.GetCachesAsync<UserProfile>(
                        userIds.Select(_cacheKeyProvider.GetUserProfileCacheKey), cancellationToken))
                    .Select(x => x.Value).ToList()!;
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to retrieve user profile caches");
                cachedUserProfiles = new List<UserProfile>();
            }

            var uncachedIds = userIds.Where(id => !cachedUserProfiles.Any(v => v.Id == id));

            if (uncachedIds.Count() > 0) {
                var userProfiles = await _userProfileRepository.GetUserProfilesAsync(uncachedIds, cancellationToken);
                await CacheUserProfilesAsync(userProfiles, cancellationToken);

                return cachedUserProfiles.Union(userProfiles).ToList();
            }

            return cachedUserProfiles.ToList();
        }

        public async Task CacheUserProfilesAsync (IEnumerable<UserProfile> userProfiles, CancellationToken cancellationToken = default) {
            try {
                var cacheDuration = TimeSpan.FromSeconds(_cachingConfigurations.UserProfileCacheDurationInSeconds);

                foreach (var userProfile in userProfiles) {
                    _cacheContext.AddCache(_cacheKeyProvider.GetUserProfileCacheKey(userProfile.Id), userProfile, cacheDuration);
                }

                await _cacheContext.CommitAsync(cancellationToken);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to add user profile caches");
            }
        }

        public async Task RemoveUserProfileCachesAsync (IEnumerable<string> userIds, CancellationToken cancellationToken = default) {
            try {
                foreach (var userId in userIds) {
                    _cacheContext.RemoveCache(_cacheKeyProvider.GetUserProfileCacheKey(userId));
                }

                await _cacheContext.CommitAsync(cancellationToken);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to remove user profile caches");
            }
        }

    }
}
