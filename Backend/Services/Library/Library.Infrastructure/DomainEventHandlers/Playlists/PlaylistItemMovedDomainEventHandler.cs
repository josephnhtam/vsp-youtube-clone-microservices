using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Playlists;
using Library.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Playlists {
    public class PlaylistItemMovedDomainEventHandler<TPlaylist> :
        IDomainEventHandler<PlaylistItemMovedDomainEvent<TPlaylist>>
        where TPlaylist : OrderedPlaylistBase<TPlaylist> {

        private readonly IMongoCollectionContext<TPlaylist> _context;

        public PlaylistItemMovedDomainEventHandler (IMongoCollectionContext<TPlaylist> context) {
            _context = context;
        }

        public Task Handle (PlaylistItemMovedDomainEvent<TPlaylist> @event, CancellationToken cancellationToken) {
            if (@event.FromPosition == @event.ToPosition) return Task.CompletedTask;

            var filter = Builders<TPlaylist>.Filter.Eq(x => x.Id, @event.PlaylistId);

            bool upToDown = @event.FromPosition < @event.ToPosition;

            var update = Builders<TPlaylist>.Update
                .Inc($"{nameof(OrderedPlaylistBase<TPlaylist>.Items)}.$[a].{nameof(OrderedPlaylistItem.Position)}", upToDown ? -1 : 1)
                .Set($"{nameof(OrderedPlaylistBase<TPlaylist>.Items)}.$[b].{nameof(OrderedPlaylistItem.Position)}", @event.ToPosition)
                .Set(nameof(PlaylistBase.UpdateDate), @event.UpdateDate);

            var arrayFilters = new[]{
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("$and", new BsonArray(new []{
                            new BsonDocument($"a.{nameof(OrderedPlaylistItem.Position)}",
                                new BsonDocument(upToDown ? "$gt" : "$lt", @event.FromPosition)
                            ),
                            new BsonDocument($"a.{nameof(OrderedPlaylistItem.Position)}",
                                new BsonDocument(upToDown ? "$lte" : "$gte", @event.ToPosition)
                            )
                        })
                    )),

                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument($"b.{nameof(OrderedPlaylistItem.Position)}",
                            new BsonDocument("$eq", @event.FromPosition)
                    )),
                };

            _context.UpdateOne(filter, update, new UpdateOptions {
                ArrayFilters = arrayFilters
            });
            return Task.CompletedTask;
        }
    }
}
