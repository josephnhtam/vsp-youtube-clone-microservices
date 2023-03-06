using Domain.Events;
using Infrastructure.TransactionalEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using SharedKernel.Exceptions;
using System.Data;

namespace Infrastructure.EFCore {
    public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext {

        private readonly TDbContext _dbContext;
        private readonly ITransactionalEventsCommitter? _transactionalEventsCommitter;
        private readonly IDomainEventsDispatcher? _domainEventsDispatcher;
        private readonly UnitOfWorkConfig _config;
        private readonly ILogger<UnitOfWork<TDbContext>> _logger;

        public UnitOfWork (TDbContext dbContext, IOptions<UnitOfWorkConfig> config, IServiceProvider serviceProvider, ILogger<UnitOfWork<TDbContext>> logger) {
            _dbContext = dbContext;
            _config = config.Value;
            _logger = logger;

            _transactionalEventsCommitter = serviceProvider.GetService<ITransactionalEventsCommitter>();
            _domainEventsDispatcher = serviceProvider.GetService<IDomainEventsDispatcher>();
        }

        public async Task CommitAsync (CancellationToken cancellationToken = default) {
            if (_domainEventsDispatcher != null) {
                await _domainEventsDispatcher.DispatchDomainEventsAsync();
            }

            var eventGroups = _transactionalEventsCommitter?.ObtainEventGroups();

            if (eventGroups?.Count > 0) {
                await ExecuteTransactionAsync(async () => {
                    await _transactionalEventsCommitter!.CommitEventsAsync(eventGroups, _config.TransactionalEventAvailableDelay);
                    await DoCommitAsync(cancellationToken);
                });
            } else {
                await DoCommitAsync(cancellationToken);
            }
        }

        public async Task DoCommitAsync (CancellationToken cancellationToken = default) {
            try {
                await _dbContext.SaveChangesAsync(cancellationToken);
            } catch (DbUpdateException ex) {
                if (ex.Identify(ExceptionCategories.UniqueViolation)) {
                    foreach (var entry in ex.Entries) {
                        entry.State = EntityState.Detached;
                    }
                }
                throw;
            }
        }

        public async Task ExecuteOptimisticTransactionAsync (Func<Task> task, Action<ITransactionOptions>? options = null, CancellationToken cancellationToken = default) {
            await ExecuteOptimisticUpdateAsync(async () => await ExecuteTransactionAsync(task, options, cancellationToken));
        }

        public async Task ExecuteTransactionAsync (Func<Task> task, Action<ITransactionOptions>? configureOptions = null, CancellationToken cancellationToken = default) {
            if (_dbContext.Database.CurrentTransaction != null) {
                await task.Invoke();
                return;
            }

            var options = new EfCoreTransactionOptions();

            if (configureOptions != null) {
                configureOptions.Invoke(options);
            }

            await _dbContext.ExecuteResilentTransaction(async () => {
                await task.Invoke();
            }, options.IsolationLevel, cancellationToken);
        }

        public void ResetContext () {
            DetachAllEntities(_dbContext);
        }

        private static void DetachAllEntities (DbContext context) {
            foreach (var entry in context.ChangeTracker.Entries()) {
                entry.State = EntityState.Detached;
            }
        }

        public async Task ExecuteOptimisticUpdateAsync (Func<Task> task) {
            int retries = _config.OptimisticConcurrencyConflictRetryCount;
            var retryPolicy = Policy.Handle<DbUpdateConcurrencyException>()
                                    .WaitAndRetryAsync(_config.OptimisticConcurrencyConflictRetryCount,
                                        (attempt) => TimeSpan.FromMilliseconds(10 * Math.Pow(2, retries)));

            bool retry;

            do {
                retry = false;
                try {
                    await task.Invoke();
                } catch (DbUpdateConcurrencyException ex) {
                    if (retries > 0) {
                        _logger.LogWarning(ex, "A conflict occured during the optimistic update. Retrying the update.");
                        ResetContext();
                        retries--;
                        retry = true;
                    } else {
                        _logger.LogError(ex, "A conflict occured during the optimistic update. Exceeded max retry count.");
                        throw new TransientException("Optimistic concurrency conflict", ex);
                    }
                }
            } while (retry);
        }

        public bool IsInTransaction () {
            return _dbContext.Database.CurrentTransaction != null;
        }

    }

    public class EfCoreTransactionOptions : ITransactionOptions {
        public IsolationLevel? IsolationLevel { get; set; }
    }
}
