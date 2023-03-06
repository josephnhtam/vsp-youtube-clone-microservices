using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;
using Users.Domain.DomainEvents;
using Users.Domain.Models;

namespace Users.Infrastructure.DomainEventHandlers.UserProfiles {
    public class UserProfileThumbnailUpdatedDomainEventHandler : IDomainEventHandler<UserProfileThumbnailUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<UserProfile> _userProfileContext;

        public UserProfileThumbnailUpdatedDomainEventHandler (IMongoCollectionContext<UserProfile> userProfileContext) {
            _userProfileContext = userProfileContext;
        }

        public Task Handle (UserProfileThumbnailUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var userProfile = @event.UserProfile;

            var filter = Builders<UserProfile>.Filter.Eq(nameof(UserProfile.Id), userProfile.Id);

            var update = Builders<UserProfile>.Update
                .Set(nameof(UserProfile.Thumbnail), userProfile.Thumbnail)
                .Set(nameof(UserProfile.Version), userProfile.Version);

            _userProfileContext.UpdateOne(filter, update);

            return Task.CompletedTask;
        }
    }
}
