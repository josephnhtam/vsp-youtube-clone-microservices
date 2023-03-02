using Microsoft.EntityFrameworkCore;
using Subscriptions.Domain.Contracts;
using Subscriptions.Domain.Models;

namespace Subscriptions.Infrastructure.Repositories {
    public class UserProfileRepository : IUserProfileRepository {

        private readonly SubscriptionsDbContext _dbContext;

        public UserProfileRepository (SubscriptionsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default) {
            await _dbContext.UserProfiles.AddAsync(userProfile, cancellationToken);
        }

        public async Task<UserProfile?> GetUserProfileAsync (string userId, CancellationToken cancellationToken = default) {
            return await _dbContext.UserProfiles.Where(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<UserProfile?> GetUserProfileByHandleAsync (string handle, CancellationToken cancellationToken = default) {
            return await _dbContext.UserProfiles.Where(x => x.Handle == handle).FirstOrDefaultAsync(cancellationToken);
        }

        public Task DeleteUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default) {
            _dbContext.UserProfiles.Remove(userProfile);
            return Task.CompletedTask;
        }

    }
}
