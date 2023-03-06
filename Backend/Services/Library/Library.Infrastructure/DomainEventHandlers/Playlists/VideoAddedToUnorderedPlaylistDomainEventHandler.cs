using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Playlists;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Playlists {
    public class VideoAddedToUnorderedPlaylistDomainEventHandler<TPlaylist> :
        IDomainEventHandler<VideoAddedToUnorderedPlaylistDomainEvent<TPlaylist>>
        where TPlaylist : UnorderedPlaylistBase<TPlaylist> {

        private readonly IMongoCollectionContext<TPlaylist> _context;

        public VideoAddedToUnorderedPlaylistDomainEventHandler (IMongoCollectionContext<TPlaylist> context) {
            _context = context;
        }

        public Task Handle (VideoAddedToUnorderedPlaylistDomainEvent<TPlaylist> @event, CancellationToken cancellationToken) {
            var filter = Builders<TPlaylist>.Filter.Eq(x => x.Id, @event.PlaylistId);

            var update = Builders<TPlaylist>.Update
                .PushEach(
                    nameof(UnorderedPlaylistBase<TPlaylist>.Items),
                    new[] { new PlaylistItem(@event.ItemId, @event.VideoId, DateTimeOffset.UtcNow) },
                    null, 0)
                .Inc(nameof(PlaylistBase.ItemsCount), 1)
                .Set(nameof(PlaylistBase.UpdateDate), @event.UpdateDate);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
