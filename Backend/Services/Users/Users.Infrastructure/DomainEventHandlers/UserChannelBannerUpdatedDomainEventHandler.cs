using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;
using Users.Domain.DomainEvents;
using Users.Domain.Models;

namespace Users.Infrastructure.DomainEventHandlers.UserProfiles {
    public class UserChannelBannerUpdatedDomainEventHandler : IDomainEventHandler<UserChannelBannerUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<UserChannel> _userChannelContext;

        public UserChannelBannerUpdatedDomainEventHandler (IMongoCollectionContext<UserChannel> userChannelContext) {
            _userChannelContext = userChannelContext;
        }

        public Task Handle (UserChannelBannerUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var userChannel = @event.UserChannel;

            var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Id), userChannel.Id);

            var update = Builders<UserChannel>.Update
                .Set(nameof(UserChannel.Banner), userChannel.Banner);

            _userChannelContext.UpdateOne(filter, update);

            return Task.CompletedTask;
        }
    }
}
