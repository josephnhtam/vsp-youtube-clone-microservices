using VideoStore.Domain.Models;

namespace VideoStore.Domain.Contracts {
    public interface IUserProfileRepository {
        Task<UserProfile?> GetUserProfileAsync (string userId, CancellationToken cancellationToken = default);
        Task AddUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default);
        Task DeleteUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default);
    }
}
