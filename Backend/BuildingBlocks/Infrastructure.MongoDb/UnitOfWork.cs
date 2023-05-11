using Domain.Contracts;
using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Infrastructure.TransactionalEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.MongoDb {
    public class UnitOfWork : IUnitOfWork {

        private readonly IMongoClientContext _context;
        private readonly ITransactionalEventsCommitter? _transactionalEventsCommitter;
        private readonly IDomainEventsDispatcher? _domainEventsDispatcher;
        private readonly UnitOfWorkConfig _config;

        public UnitOfWork (
            IMongoClientContext context,
            IOptions<UnitOfWorkConfig> config,
            IServiceProvider serviceProvider) {
            _context = context;
            _config = config.Value;

            _transactionalEventsCommitter = serviceProvider.GetService<ITransactionalEventsCommitter>();
            _domainEventsDispatcher = serviceProvider.GetService<IDomainEventsDispatcher>();
        }

        public async Task CommitAsync (CancellationToken cancellationToken = default) {
            if (_domainEventsDispatcher != null) {
                await _domainEventsDispatcher.DispatchDomainEventsAsync();
            }

            var eventGroups = _transactionalEventsCommitter?.ObtainEventGroups();

            if (eventGroups?.Count > 0) {
                await _transactionalEventsCommitter!.AddToContextAsync(eventGroups, _config.TransactionalEventAvailableDelay);
            }

            await DoCommitAsync(cancellationToken);
        }

        public async Task DoCommitAsync (CancellationToken cancellationToken = default) {
            await _context.CommitAsync(cancellationToken);
        }

        public async Task ExecuteOptimisticTransactionAsync (Func<Task> task, Action<ITransactionOptions>? options = null, CancellationToken cancellationToken = default) {
            await ExecuteOptimisticUpdateAsync(async () => await ExecuteTransactionAsync(task, options, cancellationToken));
        }

        public Task ExecuteOptimisticUpdateAsync (Func<Task> task) {
            throw new NotSupportedException();
        }

        public async Task ExecuteTransactionAsync (Func<Task> task, Action<ITransactionOptions>? configureOptions = null, CancellationToken cancellationToken = default) {
            if (_context.IsInTransaction) {
                await task.Invoke();
                return;
            }

            var options = new MongoDbTransactionOptions();

            if (configureOptions != null) {
                configureOptions.Invoke(options);
            }

            await _context.StartSessionAsync(options.SessionOptions, cancellationToken);
            await _context.ExecuteTransactionAsync(task, options.TransactionOptions, cancellationToken);
        }

        public bool IsInTransaction () {
            return _context.IsInTransaction;
        }

        public void ResetContext () {
            _context.Reset();
        }

    }

    public class MongoDbTransactionOptions : ITransactionOptions {
        public ClientSessionOptions? SessionOptions { get; set; }
        public TransactionOptions? TransactionOptions { get; set; }
    }
}
