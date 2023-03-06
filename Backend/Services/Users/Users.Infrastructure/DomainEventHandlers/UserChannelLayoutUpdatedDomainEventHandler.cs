using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;
using Users.Domain.DomainEvents;
using Users.Domain.Models;

namespace Users.Infrastructure.DomainEventHandlers.UserProfiles {
    public class UserChannelLayoutUpdatedDomainEventHandler : IDomainEventHandler<UserChannelLayoutUpdatedDomainEvent> {

        private readonly IMongoCollectionContext<UserChannel> _userChannelContext;

        public UserChannelLayoutUpdatedDomainEventHandler (IMongoCollectionContext<UserChannel> userChannelContext) {
            _userChannelContext = userChannelContext;
        }

        public Task Handle (UserChannelLayoutUpdatedDomainEvent @event, CancellationToken cancellationToken) {
            var channel = @event.Channel;

            var filter = Builders<UserChannel>.Filter.Eq(nameof(UserChannel.Id), channel.Id);

            var update = Builders<UserChannel>.Update
                .Set(nameof(UserChannel.UnsubscribedSpotlightVideoId), channel.UnsubscribedSpotlightVideoId)
                .Set(nameof(UserChannel.SubscribedSpotlightVideoId), channel.SubscribedSpotlightVideoId)
                .Set(nameof(UserChannel.Sections), channel.Sections);

            _userChannelContext.UpdateOne(filter, update);

            return Task.CompletedTask;
        }
    }
}
