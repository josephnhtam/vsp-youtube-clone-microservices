using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Library.Domain.Specifications;
using MongoDB.Driver;

namespace Library.Infrastructure.Repositories {
    public class PlaylistRefRepository : IPlaylistRefRepository {

        private readonly IMongoCollectionContext<PlaylistRef> _context;
        private readonly IDomainEventEmittersTracker _emittersTracker;

        public PlaylistRefRepository (IMongoCollectionContext<PlaylistRef> context, IDomainEventEmittersTracker emittersTracker) {
            _context = context;
            _emittersTracker = emittersTracker;
        }

        public Task AddPlaylistRefAsync (PlaylistRef playlistRef, CancellationToken cancellationToken = default) {
            _emittersTracker.Track(playlistRef);
            _context.InsertOne(playlistRef);
            return Task.CompletedTask;
        }

        public Task RemovePlaylistRefAsync (string userId, Guid playlistId, CancellationToken cancellationToken = default) {
            var filterBuilder = Builders<PlaylistRef>.Filter;
            var filter = filterBuilder.Eq(x => x.UserId, userId) & filterBuilder.Eq(x => x.PlaylistId, playlistId);
            _context.DeleteOne(filter);
            return Task.CompletedTask;
        }

        public async Task<PlaylistRef?> GetPlaylistRefAsync (string userId, Guid playlistId, CancellationToken cancellationToken = default) {
            var filterBuilder = Builders<PlaylistRef>.Filter;
            var filter = filterBuilder.Eq(x => x.UserId, userId) & filterBuilder.Eq(x => x.PlaylistId, playlistId);
            return await _context.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<PlaylistRef>> GetPlaylistRefsAsync (string userId, Pagination? pagination, CancellationToken cancellationToken = default) {
            var filter = Builders<PlaylistRef>.Filter.Eq(x => x.UserId, userId);
            var sort = Builders<PlaylistRef>.Sort.Descending(x => x.CreateDate);

            var find = _context.Collection.Find(filter).Sort(sort);

            if (pagination != null) {
                find = find
                    .Skip(Math.Max(0, pagination.Page - 1) * pagination.PageSize)
                    .Limit(pagination.PageSize);
            }

            return find.ToListAsync(cancellationToken);
        }

        public async Task<bool> HasPlaylistRefAsync (string userId, Guid playlistId, CancellationToken cancellationToken = default) {
            var filterBuilder = Builders<PlaylistRef>.Filter;
            var filter = filterBuilder.Eq(x => x.UserId, userId) & filterBuilder.Eq(x => x.PlaylistId, playlistId);
            return await _context.Collection.Find(filter).AnyAsync(cancellationToken);
        }

        public void Track (PlaylistRef playlistRef) {
            _emittersTracker.Track(playlistRef);
        }
    }

}
