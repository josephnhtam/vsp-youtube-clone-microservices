using Infrastructure.MongoDb.Contexts;
using Library.Domain.Models;
using Library.Domain.Specifications;
using Library.Infrastructure.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Library.Infrastructure.Services {
    public class PlaylistQueryHelper : IPlaylistQueryHelper {

        private readonly IMongoCollectionContext<Playlist> _playlistContext;
        private readonly IMongoCollectionContext<PlaylistRef> _playlistRefContext;

        public PlaylistQueryHelper (IMongoCollectionContext<Playlist> playlistContext, IMongoCollectionContext<PlaylistRef> playlistRefContext) {
            _playlistContext = playlistContext;
            _playlistRefContext = playlistRefContext;
        }

        public async Task<GetPlaylistsResult> GetPlaylistsAsync (string userId, bool publicOnly, int maxItemsCount, Pagination? playlistPagination, CancellationToken cancellationToken = default) {
            var filterBuilder = Builders<Playlist>.Filter;
            var filter = filterBuilder.Eq(nameof(Playlist.UserId), userId);
            if (publicOnly) filter &= filterBuilder.Eq(nameof(Playlist.Visibility), PlaylistVisibility.Public);

            var aggregate = _playlistContext.Collection.Aggregate()
                .Match(filter)
                .Sort(Builders<Playlist>.Sort.Descending(nameof(PlaylistBase.UpdateDate)))
                .Project<Playlist>(Projection.playlist(maxItemsCount));

            if (playlistPagination != null) {
                var skip = Math.Max(0, playlistPagination.Page - 1) * playlistPagination.PageSize;
                var limit = playlistPagination.PageSize;

                var resultAggregate = aggregate
                    .Group<GetPlaylistsResult>(new BsonDocument() {
                        { "_id", BsonNull.Value },
                        { "TotalCount", new BsonDocument("$count", new BsonDocument()) },
                        { "Playlists", new BsonDocument("$push", "$$ROOT") }
                    }).Project<GetPlaylistsResult>(new BsonDocument() {
                        { "TotalCount", 1 },
                        { "Playlists", new BsonDocument("$slice", new BsonArray() { "$Playlists", skip, limit }) }
                    });

                return await resultAggregate.FirstOrDefaultAsync() ??
                    new GetPlaylistsResult {
                        TotalCount = 0,
                        Playlists = new List<Playlist>()
                    };
            } else {
                var playlists = await aggregate.ToListAsync();
                return new GetPlaylistsResult {
                    TotalCount = playlists.Count,
                    Playlists = playlists
                };
            }
        }

        public async Task<GetPlaylistsResult> GetPlaylistsIncludingRefAsync (string userId, int maxItemsCount, Pagination? playlistPagination, CancellationToken cancellationToken = default) {
            var aggregate = _playlistRefContext.Collection.Aggregate()
                .Match(Builders<PlaylistRef>.Filter.Eq(x => x.UserId, userId))
                .AppendStage<BsonDocument>(
                    Lookup.Playlist(_playlistContext.Collection.CollectionNamespace.CollectionName, maxItemsCount))
                .Project(Projection.playlistRef)
                .Unwind("Playlist")
                .AppendStage<BsonDocument>(
                    new BsonDocument("$set",
                        new BsonDocument("Playlist.UpdateDate", "$CreateDate"))
                )
                .ReplaceRoot<Playlist>("$Playlist");

            var pipeline = new EmptyPipelineDefinition<Playlist>()
                .Match(Builders<Playlist>.Filter.Eq(x => x.UserId, userId))
                .Project<Playlist, Playlist, Playlist>(Projection.playlist(maxItemsCount));

            aggregate = aggregate.UnionWith(_playlistContext.Collection, pipeline);

            aggregate = aggregate
                .Sort(Builders<Playlist>.Sort.Descending(nameof(PlaylistBase.UpdateDate)));

            if (playlistPagination != null) {
                var skip = Math.Max(0, playlistPagination.Page - 1) * playlistPagination.PageSize;
                var limit = playlistPagination.PageSize;

                var resultAggregate = aggregate
                    .Group<GetPlaylistsResult>(new BsonDocument() {
                        { "_id", BsonNull.Value },
                        { "TotalCount", new BsonDocument("$count", new BsonDocument()) },
                        { "Playlists", new BsonDocument("$push", "$$ROOT") }
                    }).Project<GetPlaylistsResult>(new BsonDocument() {
                        { "TotalCount", 1 },
                        { "Playlists", new BsonDocument("$slice", new BsonArray() { "$Playlists", skip, limit }) }
                    });

                return await resultAggregate.FirstOrDefaultAsync() ??
                    new GetPlaylistsResult {
                        TotalCount = 0,
                        Playlists = new List<Playlist>()
                    };
            } else {
                var playlists = await aggregate.ToListAsync();
                return new GetPlaylistsResult {
                    TotalCount = playlists.Count,
                    Playlists = playlists
                };
            }
        }

        private static class Lookup {
            public static BsonDocument Playlist (string collectionName, int maxItemsCount) {
                return new BsonDocument("$lookup", new BsonDocument() {
                    { "from", collectionName },
                    { "as", "Playlist" },
                    { "let", new BsonDocument("playlist_id", "$PlaylistId") },
                    { "pipeline",
                        new BsonArray() {
                            new BsonDocument("$match",
                                new BsonDocument("$expr",
                                    new BsonDocument("$eq", new BsonArray() { "$_id", "$$playlist_id" })
                                )
                            ),
                            new BsonDocument("$project", Projection.playlist(maxItemsCount))
                        }
                    }
                });
            }
        }

        private static class Projection {
            public static BsonDocument playlist (int maxItemsCount) {
                var result = new BsonDocument() {
                    { nameof(Playlist.Title), 1 },
                    { nameof(Playlist.UserId), 1 },
                    { nameof(Playlist.CreateDate), 1 },
                    { nameof(Playlist.UpdateDate), 1 },
                    { nameof(Playlist.Visibility), 1 },
                    { nameof(Playlist.ItemsCount), 1 },

                };

                if (maxItemsCount > 0) {
                    result.Add(new BsonElement(nameof(Playlist.Items),
                        new BsonDocument("$filter", new BsonDocument() {
                            { "input", $"${nameof(Playlist.Items)}" },
                            { "as", "item" },
                            { "cond", new BsonDocument("$lt", new BsonArray { "$$item.Position", maxItemsCount }) }
                        })
                    ));
                }

                return result;
            }

            public static BsonDocument playlistRef = new BsonDocument() {
                { "CreateDate", 1 },
                { "Playlist", new BsonDocument("$filter",
                    new BsonDocument() {
                        { "input", "$Playlist" },
                        { "as", "pl" },
                        { "cond", new BsonDocument("$ne", new BsonArray { "$$pl.Visibility", PlaylistVisibility.Private }) }
                    })
                }
            };
        }
    }
}
