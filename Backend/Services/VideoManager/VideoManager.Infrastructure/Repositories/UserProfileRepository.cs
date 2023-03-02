using Microsoft.EntityFrameworkCore;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.Infrastructure.Repositories {
    public class UserProfileRepository : IUserProfileRepository {

        private readonly VideoManagerDbContext _dbContext;

        public UserProfileRepository (VideoManagerDbContext dbContext) {
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
