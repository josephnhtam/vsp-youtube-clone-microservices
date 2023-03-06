using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;
using Users.Domain.DomainEvents;
using Users.Domain.Models;

namespace Users.Infrastructure.DomainEventHandlers.UserProfiles {
    public class UserProfileInfoUpdatedDomainEventHandler : IDomainEventHandler<UserProfileInfoUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<UserProfile> _userProfileContext;
        private readonly IMongoCollectionContext<UserChannel> _userChannelContext;

        public UserProfileInfoUpdatedDomainEventHandler (
            IMongoCollectionContext<UserProfile> userProfileContext,
            IMongoCollectionContext<UserChannel> userChannelContext) {
            _userProfileContext = userProfileContext;
            _userChannelContext = userChannelContext;
        }

        public Task Handle (UserProfileInfoUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var userProfile = @event.UserProfile;

            {
                var filter = Builders<UserProfile>.Filter.Eq(nameof(UserProfile.Id), userProfile.Id);

                var update = Builders<UserProfile>.Update
                    .Set(nameof(UserProfile.DisplayName), userProfile.DisplayName)
                    .Set(nameof(UserProfile.Description), userProfile.Description)
                    .Set(nameof(UserProfile.Email), userProfile.Email)
                    .Set(nameof(UserProfile.Thumbnail), userProfile.Thumbnail)
                    .Set(nameof(UserProfile.Version), userProfile.Version);

                if (!string.IsNullOrEmpty(userProfile.Handle)) {
                    update = update.Set(nameof(UserProfile.Handle), userProfile.Handle);
                } else {
                    update = update.Unset(nameof(UserProfile.Handle));
                }

                _userProfileContext.UpdateOne(filter, update);
            }

            {
                var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Id), userProfile.Id);

                UpdateDefinition<UserChannel> update;

                if (!string.IsNullOrEmpty(userProfile.Handle)) {
                    update = Builders<UserChannel>.Update.Set(nameof(UserChannel.Handle), userProfile.Handle);
                } else {
                    update = Builders<UserChannel>.Update.Unset(nameof(UserChannel.Handle));
                }

                _userChannelContext.UpdateOne(filter, update);
            }

            return Task.CompletedTask;
        }
    }
}
