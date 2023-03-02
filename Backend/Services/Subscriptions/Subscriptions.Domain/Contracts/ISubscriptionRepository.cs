using Subscriptions.Domain.Models;
using Subscriptions.Domain.Specifications;

namespace Subscriptions.Domain.Contracts {
    public interface ISubscriptionRepository {
        Task<Subscription?> GetSubscriptionAsync (string userId, string targetId, bool includeUser, bool includeTarget, CancellationToken cancellationToken = default);
        Task<List<Subscription>> GetSubscriptionsAsync (string userId, bool includeTarget, SubscriptionTargetSort? sort, Pagination? pagination, CancellationToken cancellationToken = default);
        Task<List<Subscription>> GetSubscribersAsync (string userId, bool includeUser, NotificationType? notificationType, SubscriptionTargetSort? sort, Pagination? pagination, CancellationToken cancellationToken = default);
        Task AddSubscriptionAsync (Subscription subscription, CancellationToken cancellationToken = default);
        Task RemoveSubscriptionAsync (Subscription subscription, CancellationToken cancellationToken = default);
        void Update (Subscription subscription);
    }
}
