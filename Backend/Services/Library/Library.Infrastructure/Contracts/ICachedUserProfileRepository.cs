using Library.Domain.Models;

namespace Library.Infrastructure.Contracts {
    public interface ICachedUserProfileRepository {
        Task<UserProfile?> GetUserProfileAsync (string userId, CancellationToken cancellationToken = default);
        Task<List<UserProfile>> GetUserProfilesAsync (IEnumerable<string> userIds, CancellationToken cancellationToken = default);
        Task CacheUserProfilesAsync (IEnumerable<UserProfile> userProfiles, CancellationToken cancellationToken = default);
        Task RemoveUserProfileCachesAsync (IEnumerable<string> userIds, CancellationToken cancellationToken = default);
    }
}
