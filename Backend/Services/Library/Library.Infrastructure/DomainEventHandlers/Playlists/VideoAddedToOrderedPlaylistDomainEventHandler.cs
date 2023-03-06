using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Playlists;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Playlists {
    public class VideoAddedToOrderedPlaylistDomainEventHandler<TPlaylist> :
        IDomainEventHandler<VideoAddedToOrderedPlaylistDomainEvent<TPlaylist>>
        where TPlaylist : OrderedPlaylistBase<TPlaylist> {

        private readonly IMongoCollectionContext<TPlaylist> _context;

        public VideoAddedToOrderedPlaylistDomainEventHandler (IMongoCollectionContext<TPlaylist> context) {
            _context = context;
        }

        public Task Handle (VideoAddedToOrderedPlaylistDomainEvent<TPlaylist> @event, CancellationToken cancellationToken) {
            var filter = Builders<TPlaylist>.Filter.Eq(x => x.Id, @event.PlaylistId);

            var update = Builders<TPlaylist>.Update
                .Push(nameof(OrderedPlaylistBase<TPlaylist>.Items), new OrderedPlaylistItem(@event.ItemId, @event.VideoId, @event.Position, DateTimeOffset.UtcNow))
                .Inc(nameof(PlaylistBase.ItemsCount), 1)
                .Set(nameof(PlaylistBase.UpdateDate), @event.UpdateDate);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
