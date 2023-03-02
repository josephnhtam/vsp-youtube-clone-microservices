using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Library.Domain.Specifications;
using MongoDB.Driver;

namespace Library.Infrastructure.Repositories {
    public class UniquePlaylistRepository<T, TItem> : PlaylistRepository<T, TItem>, IUniquePlaylistRepository<T, TItem>
        where T : PlaylistBase<TItem>
        where TItem : PlaylistItem {

        private readonly IMongoCollectionContext<T> _context;

        public UniquePlaylistRepository (IServiceProvider services, IMongoCollectionContext<T> context, IDomainEventEmittersTracker emittersTracker) : base(services, context, emittersTracker) {
            _context = context;
        }

        public virtual async Task<T?> GetPlaylist (string userId, bool includeItems = false, bool updateLock = false, CancellationToken cancellationToken = default) {
            return await GetPlaylist(Builders<T>.Filter.Eq(x => x.UserId, userId), updateLock, includeItems, cancellationToken);
        }

        public async Task<T?> GetPlaylistIncludingVideo (string userId, Guid videoId, bool updateLock = false, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.UserId, userId),
                GetPlaylistIncludingVideoProjection(videoId),
                updateLock);
        }

        public async Task<T?> GetPlaylistIncludingItem (string userId, Guid itemId, bool updateLock = false, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.UserId, userId),
                GetPlaylistIncludingItemProjection(itemId),
                updateLock);
        }

        public async Task<bool> IsInPlaylist (string userId, Guid videoId, CancellationToken cancellationToken = default) {
            var filterBuilder = Builders<T>.Filter;

            var filter = filterBuilder.Eq(x => x.UserId, userId) &
                         filterBuilder.ElemMatch(nameof(PlaylistBase<TItem>.Items),
                            Builders<PlaylistItem>.Filter.Eq(x => x.VideoId, videoId));

            return (await _context.Collection.CountDocumentsAsync(filter, null, cancellationToken)) > 0;
        }

        public async Task<T?> GetPlaylist (string userId, Pagination? itemsPagination, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.UserId, userId),
                itemsPagination != null ? Math.Max(0, itemsPagination.Page - 1) * itemsPagination.PageSize : null,
                itemsPagination?.PageSize,
                cancellationToken);
        }

        public async Task<T?> GetPlaylist (string userId, int itemsSkip, int itemsLimit, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.UserId, userId),
                itemsSkip,
                itemsLimit,
                cancellationToken);
        }

        public async Task<int> GetPlaylistItemPosition (string userId, Guid videoId, CancellationToken cancellationToken = default) {
            return await GetPlaylistItemPosition(
                Builders<T>.Filter.Eq(x => x.UserId, userId),
                videoId,
                cancellationToken);
        }

        public override Task RemovePlaylistAsync (Guid id, CancellationToken cancellationToken = default) {
            throw new InvalidOperationException();
        }
    }
}
