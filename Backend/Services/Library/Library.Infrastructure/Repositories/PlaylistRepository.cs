using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Library.Domain.Specifications;
using Library.Infrastructure.ProjectionProviders;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Library.Infrastructure.Repositories {
    public class PlaylistRepository<T, TItem> : IPlaylistRepository<T, TItem>
        where T : PlaylistBase<TItem>
        where TItem : PlaylistItem {

        private readonly IServiceProvider _services;
        private readonly IMongoCollectionContext<T> _context;
        private readonly IDomainEventEmittersTracker _emittersTracker;

        public PlaylistRepository (IServiceProvider services, IMongoCollectionContext<T> context, IDomainEventEmittersTracker emittersTracker) {
            _services = services;
            _context = context;
            _emittersTracker = emittersTracker;
        }

        public virtual Task AddPlaylistAsync (T playlist, CancellationToken cancellationToken = default) {
            _emittersTracker.Track(playlist);
            _context.InsertOne(playlist);
            return Task.CompletedTask;
        }

        public virtual Task RemovePlaylistAsync (Guid id, CancellationToken cancellationToken = default) {
            _context.DeleteOne(Builders<T>.Filter.Eq(x => x.Id, id));
            return Task.CompletedTask;
        }

        public virtual async Task<T?> GetPlaylist (Guid id, bool includeItems = false, bool updateLock = false, CancellationToken cancellationToken = default) {
            return await GetPlaylist(Builders<T>.Filter.Eq(x => x.Id, id), updateLock, includeItems, cancellationToken);
        }

        public async Task<T?> GetPlaylistIncludingVideo (Guid id, Guid videoId, bool updateLock = false, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.Id, id),
                GetPlaylistIncludingVideoProjection(videoId),
                updateLock);
        }

        public async Task<T?> GetPlaylistIncludingItem (Guid id, Guid itemId, bool updateLock = false, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.Id, id),
                GetPlaylistIncludingItemProjection(itemId),
                updateLock);
        }

        public virtual void Track (T playlist) {
            _emittersTracker.Track(playlist);
        }

        protected virtual async Task<T?> GetPlaylist (FilterDefinition<T> filter, bool updateLock = false, bool includeItems = false, CancellationToken cancellationToken = default) {
            T? playlist;

            var projection = GetPlaylistProjection(includeItems);

            if (updateLock) {
                playlist = await _context.FindOneAndLockAsync<T>(filter, new() { Projection = projection }, cancellationToken);
            } else {
                playlist = await _context.Collection.Find(filter).Project<T>(projection).FirstOrDefaultAsync();
            }

            if (playlist != null) {
                _emittersTracker.Track(playlist);
            }

            return playlist;
        }

        public async Task<List<T>> GetPlaylistsIncludingVideo (string userId, Guid videoId, CancellationToken cancellationToken = default) {
            var filter = Builders<T>.Filter.Eq(nameof(PlaylistBase.UserId), userId);

            return await _context.Collection.Find(filter)
                .Project<T>(GetPlaylistIncludingVideoProjection(videoId))
                .ToListAsync(cancellationToken);
        }

        protected virtual ProjectionDefinition<T> GetPlaylistIncludingVideoProjection (Guid videoId) {
            var projection = Builders<T>.Projection
                .Include(nameof(PlaylistBase.UserId))
                .Include(nameof(PlaylistBase.ItemsCount))
                .Include(nameof(PlaylistBase.CreateDate))
                .Include(nameof(PlaylistBase.UpdateDate))
                .ElemMatch(nameof(PlaylistBase<TItem>.Items), Builders<PlaylistItem>.Filter.Eq(x => x.VideoId, videoId));

            AddAdditionalPlaylistProjection(ref projection);

            return projection;
        }

        protected virtual ProjectionDefinition<T> GetPlaylistIncludingItemProjection (Guid itemId) {
            var projection = Builders<T>.Projection
                .Include(nameof(PlaylistBase.UserId))
                .Include(nameof(PlaylistBase.ItemsCount))
                .Include(nameof(PlaylistBase.CreateDate))
                .Include(nameof(PlaylistBase.UpdateDate))
                .ElemMatch(nameof(PlaylistBase<TItem>.Items), Builders<PlaylistItem>.Filter.Eq(x => x.Id, itemId));

            AddAdditionalPlaylistProjection(ref projection);

            return projection;
        }

        protected virtual async Task<T?> GetPlaylist (FilterDefinition<T> filter, ProjectionDefinition<T> projection, bool updateLock = false, CancellationToken cancellationToken = default) {
            T? playlist;

            if (updateLock) {
                playlist = await _context.FindOneAndLockAsync(filter, new() { Projection = projection }, cancellationToken);
            } else {
                playlist = await _context.Collection.Find(filter).Project<T>(projection).FirstOrDefaultAsync(cancellationToken);
            }

            if (playlist != null) {
                _emittersTracker.Track(playlist);
            }

            return playlist;
        }

        protected virtual void AddAdditionalPlaylistProjection (ref ProjectionDefinition<T> projection) {
            var playlistProjections = _services.GetService(typeof(IPlaylistProjectionProvider<T>)) as IPlaylistProjectionProvider<T>;

            if (playlistProjections != null) {
                foreach (var field in playlistProjections.GetAdditionalProjectionFields()) {
                    projection = projection.Include(field);
                }
            }
        }

        protected async Task<T?> GetPlaylist (FilterDefinition<T> filter, int? skip, int? limit, CancellationToken cancellationToken = default) {
            if (!limit.HasValue || limit.Value > 0) {
                IAggregateFluent<T> aggregate = CreatePlaylistQueryAggregate(filter, skip, limit);
                return await aggregate.FirstOrDefaultAsync(cancellationToken);
            } else {
                return await GetPlaylist(filter, false, false, cancellationToken);
            }
        }

        private IAggregateFluent<T> CreatePlaylistQueryAggregate (FilterDefinition<T> filter, int? skip, int? limit) {
            var projection = new BsonDocument() {
                { nameof(PlaylistBase.UserId), 1 },
                { nameof(PlaylistBase.ItemsCount), 1 },
                { nameof(PlaylistBase.CreateDate), 1 },
                { nameof(PlaylistBase.UpdateDate), 1 }
            };

            var playlistProjections = _services.GetService(typeof(IPlaylistProjectionProvider<T>)) as IPlaylistProjectionProvider<T>;
            if (playlistProjections != null) {
                foreach (var field in playlistProjections.GetAdditionalProjectionFields()) {
                    projection.Add(field, 1);
                }
            }

            var aggregate = _context.Collection.Aggregate().Match(filter);

            if (typeof(OrderedPlaylistItem).IsAssignableFrom(typeof(TItem))) {
                aggregate = aggregate.Project<T>(
                        new BsonDocument(projection).Add(nameof(PlaylistBase<TItem>.Items),
                            new BsonDocument("$sortArray", new BsonDocument() {
                            { "input", $"${nameof(PlaylistBase<TItem>.Items)}" },
                            { "sortBy", new BsonDocument(nameof(OrderedPlaylistItem.Position), 1) }
                            })
                        )
                    );
            }

            if (skip.HasValue || limit.HasValue) {
                var slice = new BsonArray()
                    .Add($"${nameof(PlaylistBase<TItem>.Items)}")
                    .Add(skip ?? 0);

                if (limit.HasValue) {
                    slice.Add(Math.Max(1, limit.Value));
                }

                aggregate = aggregate.Project<T>(
                    new BsonDocument(projection)
                        .Add(nameof(PlaylistBase<TItem>.Items),
                            new BsonDocument("$slice", slice)
                        )
                );
            }

            return aggregate;
        }

        public async Task<T?> GetPlaylist (Guid id, Pagination? itemsPagination, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.Id, id),
                itemsPagination != null ? Math.Max(0, itemsPagination.Page - 1) * itemsPagination.PageSize : null,
                itemsPagination?.PageSize,
                cancellationToken);
        }

        public async Task<T?> GetPlaylist (Guid id, int itemsSkip, int limit, CancellationToken cancellationToken = default) {
            return await GetPlaylist(
                Builders<T>.Filter.Eq(x => x.Id, id),
                itemsSkip,
                limit,
                cancellationToken);
        }

        protected async Task<int> GetPlaylistItemPosition (FilterDefinition<T> filter, Guid videoId, CancellationToken cancellationToken = default) {
            if (typeof(OrderedPlaylistItem).IsAssignableFrom(typeof(TItem))) {
                var playlist = await _context.Collection
                    .Find(filter)
                    .Project<T>(Builders<T>.Projection.ElemMatch(
                        nameof(PlaylistBase<TItem>.Items),
                        Builders<TItem>.Filter.Eq(nameof(PlaylistItem.VideoId), videoId))
                    ).FirstOrDefaultAsync(cancellationToken);

                if (playlist != null && playlist.Items.Any()) {
                    return (playlist.Items.First() as OrderedPlaylistItem)?.Position ?? -1;
                }

                return -1;
            } else {
                var projection = new BsonDocument("Position",
                    new BsonDocument("$indexOfArray",
                        new BsonArray {
                            "$Items.VideoId",
                            new BsonBinaryData(videoId, GuidRepresentation.Standard)
                        }
                    )
                );

                var aggregate = _context.Collection.Aggregate()
                    .Match(filter)
                    .Project(projection);

                var result = await aggregate.FirstOrDefaultAsync(cancellationToken);

                if (result == null) {
                    return -1;
                }

                return result.GetElement("Position").Value.ToInt32();
            }
        }

        public async Task<List<T>> GetPlaylists (List<Guid> playlistIds, int maxItemsCount, bool publicOnly, CancellationToken cancellationToken = default) {
            var filterBuilder = Builders<T>.Filter;

            var queryTasks = playlistIds.Chunk(256).Select(idsChunk => {
                var filter = filterBuilder.In(x => x.Id, idsChunk);

                if (publicOnly) {
                    filter &= filterBuilder.Eq(nameof(Playlist.Visibility), PlaylistVisibility.Public);
                }

                if (maxItemsCount > 0) {
                    return CreatePlaylistQueryAggregate(filter, 0, maxItemsCount).ToListAsync(cancellationToken);
                } else {
                    return GetPlaylists(filter, false, cancellationToken);
                }
            }).ToList();

            await Task.WhenAll(queryTasks);

            var result = queryTasks.SelectMany(x => x.Result).ToList();

            foreach (var video in result) {
                _emittersTracker.Track(video);
            }

            return result;
        }

        protected virtual async Task<List<T>> GetPlaylists (FilterDefinition<T> filter, bool includeItems = false, CancellationToken cancellationToken = default) {
            var projection = GetPlaylistProjection(includeItems);

            List<T> playlists = await _context.Collection.Find(filter).Project<T>(projection).ToListAsync();

            foreach (var playlist in playlists) {
                _emittersTracker.Track(playlist);
            }

            return playlists;
        }

        private ProjectionDefinition<T> GetPlaylistProjection (bool includeItems) {
            var projection = Builders<T>.Projection
                .Include(nameof(PlaylistBase.UserId))
                .Include(nameof(PlaylistBase.ItemsCount))
                .Include(nameof(PlaylistBase.CreateDate))
                .Include(nameof(PlaylistBase.UpdateDate));

            if (includeItems) {
                projection = projection.Include(nameof(PlaylistBase<TItem>.Items));
            }

            AddAdditionalPlaylistProjection(ref projection);
            return projection;
        }

        public async Task<int> GetPlaylistItemPosition (Guid id, Guid videoId, CancellationToken cancellationToken = default) {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            return await GetPlaylistItemPosition(filter, videoId, cancellationToken);
        }
    }

}
