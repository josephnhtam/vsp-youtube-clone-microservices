using Domain.TransactionalEvents;
using Domain.TransactionalEvents.Contracts;
using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.TransactionalEvents.Models;
using Infrastructure.TransactionalEvents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.TransactionalEvents {
    public class TransactionalEventsContext : ITransactionalEventsContext, ITransactionalEventsCommitter {

        private string _defaultEventsGroupId;
        private Dictionary<string, List<TransactionalEvent>> _transactionalEventsGroup;

        public IServiceProvider ServiceProvider { get; private set; }

        public TransactionalEventsContext (IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider;

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

        public void CommitGroup (IMongoCollectionContext<TransactionalEventsGroup> context, string groupId, List<TransactionalEvent> transactionalEvents, TimeSpan? availableDelay = null) {
            if (transactionalEvents.Count == 0) {
                return;
            }

            DateTimeOffset now = DateTimeOffset.UtcNow;

            var filter = Builders<TransactionalEventsGroup>.Filter.Eq(x => x.Id, groupId);

            var update = Builders<TransactionalEventsGroup>.Update
                .PushEach(x => x.TransactionalEvents, transactionalEvents)
                .SetOnInsert(x => x.AvailableDate, availableDelay != null ? now + availableDelay.Value : now)
                .SetOnInsert(x => x.CreateDate, now);

            context.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public Dictionary<string, List<TransactionalEvent>> ObtainEventGroups () {
            Dictionary<string, List<TransactionalEvent>> transactionalEventsGroup;

            lock (_transactionalEventsGroup) {
                transactionalEventsGroup = new Dictionary<string, List<TransactionalEvent>>(_transactionalEventsGroup);
                _transactionalEventsGroup.Clear();
            }

            return transactionalEventsGroup;
        }

        public Task AddToContextAsync (Dictionary<string, List<TransactionalEvent>> eventGroups, TimeSpan? availableDelay = null, CancellationToken cancellationToken = default) {
            var context = ServiceProvider.GetRequiredService<IMongoCollectionContext<TransactionalEventsGroup>>();

            var groupIds = eventGroups.Keys.OrderBy(x => x);

            foreach (var groupId in groupIds) {
                CommitGroup(context, groupId, eventGroups[groupId], availableDelay);
            }

            return Task.CompletedTask;
        }

        public void RemoveFromContext (Dictionary<string, List<TransactionalEvent>> eventGroups) {
            throw new NotSupportedException();
        }

    }
}
