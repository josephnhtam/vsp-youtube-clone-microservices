using Library.Domain.Models;

namespace Library.Domain.Contracts {
    public interface IUserProfileRepository {
        Task<UserProfile?> GetUserProfileAsync (string userId, bool updateLock = false, CancellationToken cancellationToken = default);
        Task<List<UserProfile>> GetUserProfilesAsync (IEnumerable<string> userIds, CancellationToken cancellationToken = default);
        Task AddUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default);
        Task DeleteUserProfileAsync (string userId, CancellationToken cancellationToken = default);
        void Track (UserProfile userProfile);
    }
}
