using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Playlists;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Playlists {
    public class ItemRemovedFromUnorderedPlaylistDomainEventHandler<TPlaylist> :
        IDomainEventHandler<ItemRemovedFromUnorderedPlaylistDomainEvent<TPlaylist>>
        where TPlaylist : UnorderedPlaylistBase<TPlaylist> {

        private readonly IMongoCollectionContext<TPlaylist> _context;

        public ItemRemovedFromUnorderedPlaylistDomainEventHandler (IMongoCollectionContext<TPlaylist> context) {
            _context = context;
        }

        public Task Handle (ItemRemovedFromUnorderedPlaylistDomainEvent<TPlaylist> @event, CancellationToken cancellationToken) {
            RemovePlaylistItem(@event);
            return Task.CompletedTask;
        }

        private void RemovePlaylistItem (ItemRemovedFromUnorderedPlaylistDomainEvent<TPlaylist> @event) {
            var filter = Builders<TPlaylist>.Filter.Eq(x => x.Id, @event.PlaylistId);

            var update = Builders<TPlaylist>.Update
                .PullFilter(
                    nameof(UnorderedPlaylistBase<TPlaylist>.Items),
                    Builders<PlaylistItem>.Filter.Eq(nameof(PlaylistItem.Id), @event.ItemId))
                .Inc(nameof(PlaylistBase.ItemsCount), -1)
                .Set(nameof(PlaylistBase.UpdateDate), @event.UpdateDate);

            _context.UpdateOne(filter, update);
        }

    }
}
