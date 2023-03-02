using Infrastructure.MongoDb.Contexts;
using Library.Domain.Models;
using Library.Domain.Specifications;
using Library.Infrastructure.Contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Library.Infrastructure.Services {
    public class VideoQueryHelper : IVideoQueryHelper {

        private readonly IMongoCollectionContext<Video> _context;

        public VideoQueryHelper (IMongoCollectionContext<Video> context) {
            _context = context;
        }

        public async Task<GetVideosResult> GetVideosAsync (string userId, bool publicOnly, VideoSort videoSort, int? skip, int? limit, CancellationToken cancellationToken = default) {
            FilterDefinition<Video> filter = CreaterFilter(userId, publicOnly);
            SortDefinition<Video> sort = CreateSort(videoSort);

            var aggregate = _context.Collection.Aggregate()
                .Match(filter)
                .Sort(sort)
                .Group<GetVideosResult>(new BsonDocument() {
                    { "_id", BsonNull.Value },
                    { "TotalCount", new BsonDocument("$count", new BsonDocument()) },
                    { "Videos", new BsonDocument("$push", "$$ROOT") }
                });

            if (skip.HasValue || limit.HasValue) {
                var slice = new BsonArray()
                    .Add("$Videos")
                    .Add(skip ?? 0);

                if (limit.HasValue) {
                    slice.Add(limit.Value);
                }

                aggregate = aggregate.Project<GetVideosResult>(new BsonDocument() {
                    { "TotalCount", 1 },
                    { "Videos", new BsonDocument("$slice", slice) }
                });
            }

            return await aggregate.FirstOrDefaultAsync() ??
                new GetVideosResult {
                    TotalCount = 0,
                    Videos = new List<Video>()
                };
        }

        public async Task<int> GetVideoIndex (string userId, bool publicOnly, Guid videoId, VideoSort videoSort, CancellationToken cancellationToken = default) {
            FilterDefinition<Video> filter = CreaterFilter(userId, publicOnly);
            SortDefinition<Video> sort = CreateSort(videoSort);

            var aggregate = _context.Collection.Aggregate()
                .Match(filter)
                .Sort(sort)
                .Group(new BsonDocument() {
                    { "_id", BsonNull.Value },
                    { "VideoIds", new BsonDocument("$push", "$_id") }
                })
                .Project(new BsonDocument("Position",
                    new BsonDocument("$indexOfArray",
                        new BsonArray {
                            "$VideoIds",
                            new BsonBinaryData(videoId, GuidRepresentation.Standard)
                        }
                    )
                ));

            var result = await aggregate.FirstOrDefaultAsync(cancellationToken);

            if (result == null) {
                return -1;
            }

            return result.GetElement("Position").Value.ToInt32();
        }

        private static FilterDefinition<Video> CreaterFilter (string userId, bool publicOnly) {
            var filterBuilder = Builders<Video>.Filter;

            var filter = filterBuilder.Eq(nameof(Video.CreatorId), userId);

            if (publicOnly) {
                filter &= filterBuilder.Eq(nameof(Video.Visibility), VideoVisibility.Public);
            }

            return filter;
        }

        private static SortDefinition<Video> CreateSort (VideoSort videoSort) {
            var sortBuilder = Builders<Video>.Sort;

            SortDefinition<Video> sort = videoSort switch {
                VideoSort.ViewsCount => sortBuilder.Descending($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.ViewsCount)}"),
                VideoSort.LikesCount => sortBuilder.Descending($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.LikesCount)}"),
                VideoSort.CreateDate => sortBuilder.Descending(nameof(Video.CreateDate)),
                _ => sortBuilder.Descending(nameof(Video.CreateDate))
            };

            return sort;
        }

        public async Task<long> GetVideoTotalViews (string userId, bool publicOnly, CancellationToken cancellationToken = default) {
            FilterDefinition<Video> filter = CreaterFilter(userId, publicOnly);

            var aggregate = _context.Collection.Aggregate()
                .Match(filter)
                .Group(new BsonDocument() {
                    { "_id", BsonNull.Value },
                    { "TotalViews", new BsonDocument("$sum", "$Metrics.ViewsCount") }
                });

            var result = await aggregate.FirstOrDefaultAsync();

            if (result != null) {
                return result.GetElement("TotalViews").Value.AsInt64;
            }

            return 0;
        }

    }
}

