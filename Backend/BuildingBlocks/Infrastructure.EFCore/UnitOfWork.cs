using Domain.Contracts;
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
                if (IsInTransaction()) {
                    await _transactionalEventsCommitter!.AddToContextAsync(eventGroups, _config.TransactionalEventAvailableDelay);
                    await DoCommitAsync(cancellationToken);
                } else {
                    try {
                        await ExecuteTransactionAsync(async () => {
                            _transactionalEventsCommitter!.RemoveFromContext(eventGroups);
                            await _transactionalEventsCommitter!.AddToContextAsync(eventGroups, _config.TransactionalEventAvailableDelay);
                            await DoCommitAsync(false, cancellationToken);
                        }, options => {
                            (options as EfCoreTransactionOptions)!.ResetContext = false;
                        }, cancellationToken);
                    } catch (Exception) {
                        _transactionalEventsCommitter!.RemoveFromContext(eventGroups);
                        throw;
                    }

                    _dbContext.ChangeTracker.AcceptAllChanges();
                }
            } else {
                await DoCommitAsync(cancellationToken);
            }
        }

        public async Task DoCommitAsync (CancellationToken cancellationToken = default) {
            await DoCommitAsync(true, cancellationToken);
        }

        private async Task DoCommitAsync (bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
            try {
                await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
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
            if (IsInTransaction()) {
                await task.Invoke();
                return;
            }

            var options = new EfCoreTransactionOptions();

            if (configureOptions != null) {
                configureOptions.Invoke(options);
            }

            await _dbContext.ExecuteResilientTransaction(async () => {
                await task.Invoke();
            }, options.IsolationLevel, options.ResetContext, cancellationToken);
        }

        public void ResetContext () {
            _dbContext.ChangeTracker.Clear();
        }

        public async Task ExecuteOptimisticUpdateAsync (Func<Task> task) {
            int retries = _config.OptimisticConcurrencyConflictRetryCount;
            var retryPolicy = Policy.Handle<DbUpdateConcurrencyException>()
                                    .WaitAndRetryAsync(retries,
                                        (attempt) => TimeSpan.FromMilliseconds(10 * Math.Pow(2, attempt)),
                                        (ex, timespan, context) => {
                                            _logger.LogWarning(ex, $"A conflict occured during the optimistic update. Retrying the update after {timespan.Milliseconds}ms.");
                                            ResetContext();
                                        });

            try {
                await retryPolicy.ExecuteAsync(task);
            } catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, "A conflict occured during the optimistic update. Exceeded max retry count.");
                throw new TransientException("Optimistic concurrency conflict", ex);
            }
        }

        public bool IsInTransaction () {
            return _dbContext.Database.CurrentTransaction != null;
        }

    }

    public class EfCoreTransactionOptions : ITransactionOptions {
        public IsolationLevel? IsolationLevel { get; set; }
        public bool ResetContext { get; set; } = true;
    }
}
