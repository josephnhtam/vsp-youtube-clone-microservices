using History.Domain.Models;
using Infrastructure.MongoDb;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace History.Infrastructure.CollectionSeeders {
    public class VideoCollectionSeeder : MongoCollectionSeeder<Video> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<Video> collection, ILogger<IMongoCollection<Video>> logger) {
            var indexBuilder = Builders<Video>.IndexKeys;

            var indexes = new List<CreateIndexModel<Video>> {
                new (indexBuilder.Ascending(nameof(Video.CreatorId))),
                new (indexBuilder.Ascending(nameof(Video.Status))),
                new (indexBuilder.Ascending($"{nameof(Video.Metrics)}.{nameof(VideoMetrics.NextSyncDate)}")),
                new (indexBuilder.Ascending("__selection_token")),
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
