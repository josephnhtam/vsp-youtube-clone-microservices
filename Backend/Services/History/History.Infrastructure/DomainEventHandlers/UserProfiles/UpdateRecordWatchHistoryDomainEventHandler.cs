using Domain.Events;
using History.Domain.DomainEvents.UserProfiles;
using History.Domain.Models;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;

namespace History.Infrastructure.DomainEventHandlers.UserProfiles {
    public class UpdateRecordWatchHistoryDomainEventHandler : IDomainEventHandler<UpdateRecordWatchHistoryDomainEvent> {

        private readonly IMongoCollectionContext<UserProfile> _context;

        public UpdateRecordWatchHistoryDomainEventHandler (IMongoCollectionContext<UserProfile> context) {
            _context = context;
        }

        public Task Handle (UpdateRecordWatchHistoryDomainEvent @event, CancellationToken cancellationToken) {
            var filterBuilder = Builders<UserProfile>.Filter;

            var filter = filterBuilder.Eq(nameof(UserProfile.Id), @event.Id);

            var update = Builders<UserProfile>.Update
                .Set(nameof(UserProfile.RecordWatchHistory), @event.RecordWatchHistory);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
