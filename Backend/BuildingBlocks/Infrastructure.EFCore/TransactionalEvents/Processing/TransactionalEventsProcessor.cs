using Domain.TransactionalEvents.Contracts;
using Infrastructure.EFCore.TransactionalEvents.Models;
using Infrastructure.TransactionalEvents.Processing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;

namespace Infrastructure.EFCore.TransactionalEvents.Processing {
    public class TransactionalEventsProcessor<TDbContext> : ITransactionalEventsProcessor where TDbContext : DbContext {

        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<ITransactionalEventsHandler> _handlers;
        private readonly ILogger<TransactionalEventsProcessor<TDbContext>> _logger;
        private readonly TransactionalEventsProcessorConfiguration _config;
        private readonly ITransactionalEventsCommandResolver _commandResolver;

        private static SemaphoreSlim _contextSemaphore;

        static TransactionalEventsProcessor () {
            _contextSemaphore = new SemaphoreSlim(Math.Min(8, Environment.ProcessorCount));
        }

        public TransactionalEventsProcessor (
            IServiceProvider serviceProvider,
            IEnumerable<ITransactionalEventsHandler> handlers,
            IOptions<TransactionalEventsProcessorConfiguration> config,
            IOptions<TransactionalEventsContextConfig> _contextConfig,
            ILogger<TransactionalEventsProcessor<TDbContext>> logger) {
            _serviceProvider = serviceProvider;
            _handlers = handlers;
            _config = config.Value;
            _commandResolver = _contextConfig.Value.CommandResolver;
            _logger = logger;

            CheckDependencies();
        }

        private void CheckDependencies () {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<TDbContext>();
            var repository = scope.ServiceProvider.GetService<ITransactionalEventsContext>();

            if (dbContext == null)
                throw new ArgumentNullException($"Db context {nameof(TDbContext)} is not added");

            if (repository == null)
                throw new ArgumentNullException($"Transactional event repository is not added");
        }

        public async Task ProcessTransactionalEvents (CancellationToken cancellationToken = default) {
            using var suppressScope = SuppressInstrumentationScope.Begin();

            using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () => {
                using var transaction = await dbContext.Database.BeginTransactionAsync();

                try {
                    var eventsGroups = await PollEventsGroups(dbContext, _config.FetchCount);

                    if (eventsGroups.Count > 0) {
                        var processings = eventsGroups.Select(
                            eventGroup => ProcessEventsGroup(dbContext, eventGroup, cancellationToken)).ToList();

                        await Task.WhenAll(processings);

                        await dbContext.SaveChangesAsync(cancellationToken);
                    }

                    await transaction.CommitAsync();
                } catch (Exception) {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        private async Task ProcessEventsGroup (TDbContext dbContext, TransactionalEventsGroup group, CancellationToken cancellationToken = default) {
            try {
                var events = await GetEvents(
                    group.Id, group.LastSequenceNumber, cancellationToken);

                if (events.Count > 0) {
                    await Task.WhenAll(
                        _handlers.Select(
                            handler => handler.ProcessTransactionalEventsAsync(events.Select(x => x.Event).ToList(),
                            cancellationToken)));
                }

                dbContext.Remove(group);

                _logger.LogDebug("Events group ({GroupId}) is processed successfully", group.Id);
            } catch (Exception ex) {
                group.AvailableDate = DateTime.Now + TimeSpan.FromSeconds(_config.RetryDelaySeconds);
                _logger.LogError(ex, "An error occurred when processing events group ({GroupId}). Processing will be retried later.", group.Id);
            }
        }

        private async Task<List<TransactionalEventData>> GetEvents (string groupId, long sequenceNumberTo, CancellationToken cancellationToken = default) {
            try {
                await _contextSemaphore.WaitAsync();
                using (var scope = _serviceProvider.CreateScope()) {
                    using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                    return await dbContext.Set<TransactionalEventData>()
                        .Where(x => x.GroupId == groupId && x.SequenceNumber <= sequenceNumberTo)
                        .OrderBy(x => x.SequenceNumber)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);
                }
            } finally {
                _contextSemaphore.Release();
            }
        }

        private async Task<List<TransactionalEventsGroup>> PollEventsGroups (TDbContext dbContext, int fetchCount) {
            return await _commandResolver.PollEventsGroups(dbContext, fetchCount).ToListAsync();
        }

    }
}
