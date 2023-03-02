using Microsoft.EntityFrameworkCore;
using Subscriptions.Domain.Contracts;
using Subscriptions.Domain.Models;
using Subscriptions.Domain.Specifications;

namespace Subscriptions.Infrastructure.Repositories {
    public class SubscriptionRepository : ISubscriptionRepository {

        private readonly SubscriptionsDbContext _dbContext;

        public SubscriptionRepository (SubscriptionsDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<Subscription?> GetSubscriptionAsync (string userId, string targetId, bool includeUser, bool includeTarget, CancellationToken cancellationToken = default) {
            var query = _dbContext.Subscriptions.Where(x => x.UserId == userId && x.TargetId == targetId);

            if (includeUser) {
                query = query.Include(x => x.User);
            }

            if (includeTarget) {
                query = query.Include(x => x.Target);

                if (includeUser) {
                    query = query.AsSplitQuery();
                }
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync (string userId, bool includeTarget, SubscriptionTargetSort? sort, Pagination? pagination, CancellationToken cancellationToken = default) {
            var query = _dbContext.Subscriptions.Where(x => x.UserId == userId);

            if (includeTarget) {
                query = query.Include(x => x.Target);
            }

            if (sort.HasValue) {
                switch (sort) {
                    case SubscriptionTargetSort.SubscriptionDate:
                        query = query.OrderBy(x => x.SubscriptionDate);
                        break;

                    default:
                        query = query.OrderBy(x => x.Target.DisplayName);
                        break;
                }
            }

            if (pagination != null) {
                query = query
                    .Skip(Math.Max(pagination.Page - 1, 0) * pagination.PageSize)
                    .Take(pagination.PageSize);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<Subscription>> GetSubscribersAsync (string userId, bool includeUser, NotificationType? notificationType, SubscriptionTargetSort? sort, Pagination? pagination, CancellationToken cancellationToken = default) {
            var query = _dbContext.Subscriptions.Where(x => x.TargetId == userId);

            if (includeUser) {
                query = query.Include(x => x.User);
            }

            if (notificationType.HasValue) {
                query = query.Where(x => x.NotificationType == notificationType);
            }

            if (sort.HasValue) {
                switch (sort) {
                    case SubscriptionTargetSort.SubscriptionDate:
                        query = query.OrderBy(x => x.SubscriptionDate);
                        break;

                    default:
                        query = query.OrderBy(x => x.Target.DisplayName);
                        break;
                }
            }

            if (pagination != null) {
                query = query
                    .Skip(Math.Max(pagination.Page - 1, 0) * pagination.PageSize)
                    .Take(pagination.PageSize);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public void Update (Subscription subscription) {
            _dbContext.Update(subscription);
        }

        public async Task AddSubscriptionAsync (Subscription subscription, CancellationToken cancellationToken = default) {
            if (!_dbContext.Database.IsNpgsql()) {
                throw new NotSupportedException();
            }

            if (_dbContext.Database.CurrentTransaction == null) {
                throw new InvalidOperationException("Transaction is required for this operation");
            }

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ""UserProfiles"" SET ""SubscribersCount"" = ""SubscribersCount"" + 1 WHERE ""Id"" = {subscription.TargetId}",
                cancellationToken);

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ""UserProfiles"" SET ""SubscriptionsCount"" = ""SubscriptionsCount"" + 1 WHERE ""Id"" = {subscription.UserId}",
                cancellationToken);

            await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
        }

        public async Task RemoveSubscriptionAsync (Subscription subscription, CancellationToken cancellationToken = default) {
            if (!_dbContext.Database.IsNpgsql()) {
                throw new NotSupportedException();
            }

            if (_dbContext.Database.CurrentTransaction == null) {
                throw new InvalidOperationException("Transaction is required for this operation");
            }

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ""UserProfiles"" SET ""SubscribersCount"" = ""SubscribersCount"" - 1 WHERE ""Id"" = {subscription.TargetId}",
                cancellationToken);

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $@"UPDATE ""UserProfiles"" SET ""SubscriptionsCount"" = ""SubscriptionsCount"" - 1 WHERE ""Id"" = {subscription.UserId}",
                cancellationToken);

            _dbContext.Remove(subscription);
        }

    }
}
