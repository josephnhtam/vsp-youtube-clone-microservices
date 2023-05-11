using Domain.TransactionalEvents.Contracts;
using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.TransactionalEvents.Models;
using Infrastructure.TransactionalEvents.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenTelemetry;

namespace Infrastructure.MongoDb.TransactionalEvents.Processing {
    public class TransactionalEventsProcessor : ITransactionalEventsProcessor {

        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<ITransactionalEventsHandler> _handlers;
        private readonly ILogger<TransactionalEventsProcessor> _logger;
        private readonly TransactionalEventsProcessorConfiguration _config;

        private readonly TimeSpan EventsGroupSelectionTimeout = TimeSpan.FromSeconds(60);

        public TransactionalEventsProcessor (
            IServiceProvider serviceProvider,
            IEnumerable<ITransactionalEventsHandler> handlers,
            IOptions<TransactionalEventsProcessorConfiguration> config,
            ILogger<TransactionalEventsProcessor> logger) {
            _serviceProvider = serviceProvider;
            _handlers = handlers;
            _config = config.Value;
            _logger = logger;

            CheckDependencies();
        }

        private void CheckDependencies () {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<IMongoCollectionContext<TransactionalEventsGroup>>();
            var repository = scope.ServiceProvider.GetService<ITransactionalEventsContext>();

            if (context == null)
                throw new ArgumentNullException($"Collection context {nameof(IMongoClientContext)} is not added");

            if (repository == null)
                throw new ArgumentNullException($"Transactional event repository is not added");
        }

        public async Task ProcessTransactionalEvents (CancellationToken cancellationToken = default) {
            using var suppressScope = SuppressInstrumentationScope.Begin();

            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<IMongoCollectionContext<TransactionalEventsGroup>>();
            var clientContext = context.ClientContext;

            var selectionToken = await PollEventsGroupIds(context, _config.FetchCount, cancellationToken);

            if (selectionToken != null) {
                await clientContext.ExecuteTransactionAsync(async () => {
                    var groups = await RetrieveEventsGroups(context, selectionToken, cancellationToken);
                    var processings = groups.Select(group => ProcessEventsGroup(context, group, cancellationToken));

                    await Task.WhenAll(processings);

                    await clientContext.CommitAsync(cancellationToken);
                }, null, cancellationToken);
            }
        }

        private async Task ProcessEventsGroup (IMongoCollectionContext<TransactionalEventsGroup> context, TransactionalEventsGroup group, CancellationToken cancellationToken = default) {
            try {
                await Task.WhenAll(
                    _handlers.Select(handler => handler.ProcessTransactionalEventsAsync(group.TransactionalEvents, cancellationToken)));

                context.DeleteOne(Builders<TransactionalEventsGroup>.Filter.Eq(x => x.Id, group.Id));

                _logger.LogTrace("Events group ({GroupId}) is processed successfully", group.Id);
            } catch (Exception ex) {
                context.UpdateOne(
                    Builders<TransactionalEventsGroup>.Filter.Eq(x => x.Id, group.Id),
                    Builders<TransactionalEventsGroup>.Update.Set(x => x.AvailableDate,
                        DateTimeOffset.Now + TimeSpan.FromSeconds(_config.RetryDelaySeconds))
                );

                _logger.LogError(ex, "An error occurred when processing events group ({GroupId}). Processing will be retried later.", group.Id);
            }
        }

        private async Task<List<TransactionalEventsGroup>> RetrieveEventsGroups (IMongoCollectionContext<TransactionalEventsGroup> context, string selectionToken, CancellationToken cancellationToken) {
            var filter = Builders<TransactionalEventsGroup>.Filter.Eq("__selection_token", selectionToken);
            var update = Builders<TransactionalEventsGroup>.Update.Set("__concurrency_token", selectionToken);

            // Lock the selected events group
            await context.Collection.UpdateManyAsync(context.CurrentSession, filter, update, null, cancellationToken);

            return await context.Collection
                .Find(context.CurrentSession, Builders<TransactionalEventsGroup>.Filter.Eq("__selection_token", selectionToken))
                .ToListAsync(cancellationToken);
        }

        private async Task<string?> PollEventsGroupIds (IMongoCollectionContext<TransactionalEventsGroup> context, int fetchCount, CancellationToken cancellationToken = default) {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;

            var filterBuilder = Builders<TransactionalEventsGroup>.Filter;

            var filter = filterBuilder.Lt(x => x.AvailableDate, currentTime);

            bool retry;
            do {
                retry = false;

                var findResult = (await context.Collection
                        .Find(filter)
                        .Project(Builders<TransactionalEventsGroup>.Projection.Include(x => x.Id))
                        .SortBy(x => x.AvailableDate)
                        .Limit(fetchCount)
                        .ToListAsync(cancellationToken)
                    ).Select(x => (string)x.GetElement(0).Value).ToList();

                if (findResult.Count > 0) {
                    var selectionToken = Guid.NewGuid().ToString();

                    var update = Builders<TransactionalEventsGroup>.Update
                        .Set(x => x.AvailableDate, currentTime + EventsGroupSelectionTimeout)
                        .Set("__selection_token", selectionToken);

                    var updateResult = await context.Collection.UpdateManyAsync(
                        filterBuilder.In(x => x.Id, findResult) & filter,
                        update);

                    _logger.LogTrace("Selected events group {SelectedGroupCount} out of {GroupCount}", updateResult.ModifiedCount, findResult.Count);

                    if (updateResult.ModifiedCount > 0) {
                        return selectionToken;
                    } else {
                        retry = true;
                    }
                }
            } while (retry);

            return null;
        }

    }
}
