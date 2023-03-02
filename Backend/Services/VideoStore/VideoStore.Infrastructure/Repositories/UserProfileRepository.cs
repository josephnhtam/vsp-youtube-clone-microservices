using Microsoft.EntityFrameworkCore;
using VideoStore.Domain.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.Infrastructure.Repositories {
    public class UserProfileRepository : IUserProfileRepository {

        private readonly VideoStoreDbContext _dbContext;

        public UserProfileRepository (VideoStoreDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<UserProfile?> GetUserProfileAsync (string userId, CancellationToken cancellationToken = default) {
            return await _dbContext.UserProfiles.Where(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default) {
            await _dbContext.UserProfiles.AddAsync(userProfile, cancellationToken);
        }

        public Task DeleteUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default) {
            _dbContext.UserProfiles.Remove(userProfile);
            return Task.CompletedTask;
        }

    }
}
