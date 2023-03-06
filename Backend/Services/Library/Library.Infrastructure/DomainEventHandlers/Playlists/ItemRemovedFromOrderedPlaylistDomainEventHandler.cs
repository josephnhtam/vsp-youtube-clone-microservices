using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Playlists;
using Library.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Playlists {
    public class ItemRemovedFromOrderedPlaylistDomainEventHandler<TPlaylist> :
        IDomainEventHandler<ItemRemovedFromOrderedPlaylistDomainEvent<TPlaylist>>
        where TPlaylist : OrderedPlaylistBase<TPlaylist> {

        private readonly IMongoCollectionContext<TPlaylist> _context;

        public ItemRemovedFromOrderedPlaylistDomainEventHandler (IMongoCollectionContext<TPlaylist> context) {
            _context = context;
        }

        public Task Handle (ItemRemovedFromOrderedPlaylistDomainEvent<TPlaylist> @event, CancellationToken cancellationToken) {
            RemovePlaylistItem(@event);
            UpdatePlaylistItemsPosition(@event);
            return Task.CompletedTask;
        }

        private void RemovePlaylistItem (ItemRemovedFromOrderedPlaylistDomainEvent<TPlaylist> @event) {
            var filter = Builders<TPlaylist>.Filter.Eq(x => x.Id, @event.PlaylistId);

            var update = Builders<TPlaylist>.Update
                .PullFilter(
                    nameof(OrderedPlaylistBase<TPlaylist>.Items),
                    Builders<PlaylistItem>.Filter.Eq(nameof(PlaylistItem.Id), @event.ItemId))
                .Inc(nameof(PlaylistBase.ItemsCount), -1);

            _context.UpdateOne(filter, update);
        }

        private void UpdatePlaylistItemsPosition (ItemRemovedFromOrderedPlaylistDomainEvent<TPlaylist> @event) {
            var filter = Builders<TPlaylist>.Filter.Eq(x => x.Id, @event.PlaylistId);

            var arrayFilters = new[]{
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument($"a.{nameof(OrderedPlaylistItem.Position)}",
                        new BsonDocument("$gt", @event.Position)
                    )
                )
            };

            var update = Builders<TPlaylist>.Update
                .Inc($"{nameof(OrderedPlaylistBase<TPlaylist>.Items)}.$[a].{nameof(OrderedPlaylistItem.Position)}", -1)
                .Set(nameof(PlaylistBase.UpdateDate), @event.UpdateDate);

            _context.UpdateOne(filter, update, new UpdateOptions {
                ArrayFilters = arrayFilters
            });
        }
    }
}
