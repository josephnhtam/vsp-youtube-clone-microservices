using History.Domain.Contracts;
using History.Domain.Models;
using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using MongoDB.Driver;

namespace History.Infrastructure.Repositories {
    public class VideoRepository : IVideoRepository {

        private readonly IMongoCollectionContext<Video> _context;
        private readonly IDomainEventEmittersTracker _emittersTracker;

        public VideoRepository (IMongoCollectionContext<Video> context, IDomainEventEmittersTracker emittersTracker) {
            _context = context;
            _emittersTracker = emittersTracker;
        }

        public Task AddVideoAsync (Video video, CancellationToken cancellationToken = default) {
            _emittersTracker.Track(video);
            _context.InsertOne(video);
            return Task.CompletedTask;
        }

        public Task RemoveVideoAsync (Guid videoId, CancellationToken cancellationToken = default) {
            _context.DeleteOne(Builders<Video>.Filter.Eq(x => x.Id, videoId));
            return Task.CompletedTask;
        }

        public async Task<Video?> GetVideoByIdAsync (Guid id, bool updateLock = false, CancellationToken cancellationToken = default) {
            Video? video;

            var filter = Builders<Video>.Filter.Eq(nameof(Video.Id), id);

            if (updateLock) {
                video = await _context.FindOneAndLockAsync(filter, null, cancellationToken);
            } else {
                video = await _context.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            }

            if (video != null) {
                _emittersTracker.Track(video);
            }

            return video;
        }

        public async Task<List<Video>> GetVideosAsync (IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
            var filterBuilder = Builders<Video>.Filter;

            var queryTasks = ids.Chunk(256).Select(idsChunk => {
                var filter = filterBuilder.In(nameof(Video.Id), idsChunk);
                return _context.Collection.Find(filter).ToListAsync(cancellationToken);
            }).ToList();

            await Task.WhenAll(queryTasks);

            var result = queryTasks.SelectMany(x => x.Result).ToList();

            foreach (var video in result) {
                _emittersTracker.Track(video);
            }

            return result;
        }

        public async Task<List<Video>> PollVideoForMetricsSyncAsync (int fetchCount, CancellationToken cancellationToken = default) {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;

            var filterBuilder = Builders<Video>.Filter;
            var filter = filterBuilder.Ne(x => x.Metrics.NextSyncDate, null) &
                         filterBuilder.Lt(x => x.Metrics.NextSyncDate, currentTime);

            bool retry;
            do {
                retry = false;

                var findResult = await _context.Collection
                    .Find(filter)
                    .Project(Builders<Video>.Projection.Include(x => x.Id))
                    .SortBy(x => x.Metrics.NextSyncDate)
                    .Limit(fetchCount)
                    .ToListAsync();

                if (findResult.Count > 0) {
                    var selectionToken = Guid.NewGuid().ToString();

                    var update = Builders<Video>.Update
                        .Set(x => x.Metrics.NextSyncDate, null)
                        .Set("__selection_token", selectionToken);

                    var updateResult = await _context.Collection.UpdateManyAsync(
                        filterBuilder.In(x => x.Id, findResult.Select(x => (Guid)x.GetElement(0).Value)) & filter,
                        update);

                    if (updateResult.ModifiedCount > 0) {
                        var projection = Builders<Video>.Projection
                            .Include(nameof(Video.Metrics))
                            .Include(nameof(Video.Status))
                            .Include(nameof(Video.Visibility));

                        return await _context.Collection
                            .Find(filterBuilder.Eq("__selection_token", selectionToken))
                            .Project<Video>(projection)
                            .ToListAsync();
                    } else {
                        retry = true;
                    }
                }
            } while (retry);

            return new List<Video>();
        }

        public void Track (Video video) {
            _emittersTracker.Track(video);
        }

    }
}
