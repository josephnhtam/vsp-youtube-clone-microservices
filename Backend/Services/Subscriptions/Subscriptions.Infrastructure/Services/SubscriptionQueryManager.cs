using Microsoft.EntityFrameworkCore;

namespace Subscriptions.Infrastructure.Services {
    public class SubscriptionQueryManager : ISubscriptionQueryManager {

        private readonly SubscriptionsDbContext _dbContext;

        public SubscriptionQueryManager (SubscriptionsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<List<string>?> GetSubscriptionTargetIdsAsync (string userId, CancellationToken cancellationToken = default) {
            return await _dbContext.Subscriptions
                .Where(x => x.UserId == userId)
                .Select(x => x.TargetId)
                .ToListAsync(cancellationToken);
        }

    }
}
