namespace Subscriptions.Infrastructure.Services {
    public interface ISubscriptionQueryManager {
        Task<List<string>?> GetSubscriptionTargetIdsAsync (string userId, CancellationToken cancellationToken = default);
    }
}
