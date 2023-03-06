using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;
using Users.Domain.DomainEvents;
using Users.Domain.Models;

namespace Users.Infrastructure.DomainEventHandlers.UserProfiles {
    public class UserProfileStatusUpdatedDomainEventHandler : IDomainEventHandler<UserProfileStatusUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<UserProfile> _context;

        public UserProfileStatusUpdatedDomainEventHandler (IMongoCollectionContext<UserProfile> context) {
            _context = context;
        }

        public Task Handle (UserProfileStatusUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var filterBuilder = Builders<UserProfile>.Filter;

            var userProfile = @event.UserProfile;

            var filter = filterBuilder.Eq(nameof(UserProfile.Id), userProfile.Id);

            var update = Builders<UserProfile>.Update
                .Set(nameof(UserProfile.Status), userProfile.Status)
                .Set(nameof(UserProfile.Version), userProfile.Version);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
