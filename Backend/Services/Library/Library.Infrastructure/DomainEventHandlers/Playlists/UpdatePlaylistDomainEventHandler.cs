using Domain.Events;
using Infrastructure.MongoDb.Contexts;
using Library.Domain.DomainEvents.Playlists;
using Library.Domain.Models;
using MongoDB.Driver;

namespace Library.Infrastructure.DomainEventHandlers.Playlists {
    public class UpdatePlaylistDomainEventHandler : IDomainEventHandler<UpdatePlaylistDomainEvent> {

        private readonly IMongoCollectionContext<Playlist> _context;

        public UpdatePlaylistDomainEventHandler (IMongoCollectionContext<Playlist> context) {
            _context = context;
        }

        public Task Handle (UpdatePlaylistDomainEvent @event, CancellationToken cancellationToken) {
            var filter = Builders<Playlist>.Filter.Eq(x => x.Id, @event.PlaylistId);

            List<UpdateDefinition<Playlist>> updates = new List<UpdateDefinition<Playlist>>();

            if (@event.Title != null) {
                updates.Add(Builders<Playlist>.Update.Set(nameof(Playlist.Title), @event.Title));
            }

            if (@event.Description != null) {
                updates.Add(Builders<Playlist>.Update.Set(nameof(Playlist.Description), @event.Description));
            }

            if (@event.Visibility != null) {
                updates.Add(Builders<Playlist>.Update.Set(nameof(Playlist.Visibility), @event.Visibility));
            }

            var update = Builders<Playlist>.
                Update.Combine(updates)
                .Set(nameof(PlaylistBase.UpdateDate), @event.UpdateDate);

            _context.UpdateOne(filter, update);
            return Task.CompletedTask;
        }
    }
}
