using Infrastructure.MongoDb;
using Library.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Infrastructure.CollectionSeeders {
    public class DislikedPlaylistCollectionSeeder : MongoCollectionSeeder<DislikedPlaylist> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<DislikedPlaylist> collection, ILogger<IMongoCollection<DislikedPlaylist>> logger) {
            var indexBuilder = Builders<DislikedPlaylist>.IndexKeys;

            var indexes = new List<CreateIndexModel<DislikedPlaylist>> {
                new (indexBuilder.Ascending(nameof(DislikedPlaylist.UserId)), new (){ Unique = true }),
                new (indexBuilder.Ascending($"{nameof(DislikedPlaylist.Items)}.{nameof(PlaylistItem.Id)}")),
                new (indexBuilder.Ascending($"{nameof(DislikedPlaylist.Items)}.{nameof(PlaylistItem.VideoId)}")),
                new (indexBuilder.Ascending($"{nameof(DislikedPlaylist.Items)}.{nameof(PlaylistItem.CreateDate)}"))
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
