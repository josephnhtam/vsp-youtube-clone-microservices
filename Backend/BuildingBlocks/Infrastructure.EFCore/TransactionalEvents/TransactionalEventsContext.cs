using Domain.TransactionalEvents;
using Domain.TransactionalEvents.Contracts;
using Infrastructure.EFCore.TransactionalEvents.Models;
using Infrastructure.TransactionalEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.EFCore.TransactionalEvents {
    public class TransactionalEventsContext<TDbContext> : ITransactionalEventsContext, ITransactionalEventsCommitter where TDbContext : DbContext {

        private readonly ITransactionalEventsCommandResolver _commandResolver;
        private readonly TransactionalEventsContextConfig _config;
        private readonly ILogger<TransactionalEventsContext<TDbContext>> _logger;

        private string _defaultEventsGroupId;
        private Dictionary<string, List<TransactionalEvent>> _transactionalEventsGroup;

        public IServiceProvider ServiceProvider { get; private set; }

        public TransactionalEventsContext (
            IServiceProvider serviceProvider,
            IOptions<TransactionalEventsContextConfig> config,
            ILogger<TransactionalEventsContext<TDbContext>> logger) {
            ServiceProvider = serviceProvider;
            _config = config.Value;
            _commandResolver = _config.CommandResolver;
            _logger = logger;

            _transactionalEventsGroup = new Dictionary<string, List<TransactionalEvent>>();
            ResetDefaultEventsGroudId();
        }

        public void ResetDefaultEventsGroudId () {
            _defaultEventsGroupId = Guid.NewGuid().ToString();
        }

        public void AddEvent (TransactionalEvent message) {
            AddEvent(_defaultEventsGroupId, message);
        }

        public void AddEvent (string? groupId, TransactionalEvent message) {
            if (string.IsNullOrWhiteSpace(groupId)) {
                groupId = _defaultEventsGroupId;
            }

            lock (_transactionalEventsGroup) {
                if (!_transactionalEventsGroup.TryGetValue(groupId, out var eventList)) {
                    eventList = new List<TransactionalEvent>();
                    _transactionalEventsGroup[groupId] = eventList;
                }
                eventList.Add(message);
            }
        }

        public bool HasPendingEvent () {
            lock (_transactionalEventsGroup) {
                return _transactionalEventsGroup.Count > 0;
            }
        }

        public void ClearUncommitedEvents () {
            lock (_transactionalEventsGroup) {
                _transactionalEventsGroup.Clear();
            }
        }

        public bool HasPendingEvents () {
            lock (_transactionalEventsGroup) {
                return _transactionalEventsGroup.Count > 0;
            }
        }

        private async Task AddGroupAsync (TDbContext context, string groupId, List<TransactionalEvent> transactionalEvents, TimeSpan? availableDelay, CancellationToken cancellationToken) {
            if (transactionalEvents.Count == 0) {
                return;
            }

            await UpsertTransactionalEventsGroup(context, groupId, transactionalEvents, availableDelay, cancellationToken);
        }

        private async Task UpsertTransactionalEventsGroup (TDbContext context, string groupId, List<TransactionalEvent> transactionalEvents, TimeSpan? availableDelay, CancellationToken cancellationToken) {
            try {
                long postUpdateLastSequenceNumber = await _commandResolver.UpsertEventsGroupAndGetLastSequenceNumber(
                    context, groupId, transactionalEvents.Count,
                                DateTimeOffset.UtcNow, availableDelay ?? TimeSpan.Zero);

                var nextSequenceNumber = postUpdateLastSequenceNumber - transactionalEvents.Count + 1;

                var transactionalEventDatas = transactionalEvents.Select((@event, offset) =>
                    new TransactionalEventData {
                        GroupId = groupId,
                        SequenceNumber = nextSequenceNumber + offset,
                        Event = @event
                    }).ToList();

                await context.AddRangeAsync(transactionalEventDatas);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to upsert transactional events group ({GroupId})", groupId);
                throw;
            }
        }

        public Dictionary<string, List<TransactionalEvent>> ObtainEventGroups () {
            Dictionary<string, List<TransactionalEvent>> transactionalEventsGroup;

            lock (_transactionalEventsGroup) {
                transactionalEventsGroup = new Dictionary<string, List<TransactionalEvent>>(_transactionalEventsGroup);
                _transactionalEventsGroup.Clear();
            }

            return transactionalEventsGroup;
        }

        public async Task AddToContextAsync (Dictionary<string, List<TransactionalEvent>> eventGroups, TimeSpan? availableDelay = null, CancellationToken cancellationToken = default) {
            var context = ServiceProvider.GetRequiredService<TDbContext>();

            var groupIds = eventGroups.Keys.OrderBy(x => x);

            try {
                foreach (var groupId in groupIds) {
                    await AddGroupAsync(context, groupId, eventGroups[groupId], availableDelay, cancellationToken);
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "An error occurred when committing events group");
                throw;
            }
        }

        public void RemoveFromContext (Dictionary<string, List<TransactionalEvent>> eventGroups) {
            var context = ServiceProvider.GetRequiredService<TDbContext>();

            var events = eventGroups.SelectMany(x => x.Value).ToList();

            var dataEntries = context.ChangeTracker.Entries().Where(e =>
                e.Entity is TransactionalEventData data && events.Contains(data.Event)
            );

            foreach (var entry in dataEntries) {
                entry.State = EntityState.Detached;
            }
        }

    }
}
