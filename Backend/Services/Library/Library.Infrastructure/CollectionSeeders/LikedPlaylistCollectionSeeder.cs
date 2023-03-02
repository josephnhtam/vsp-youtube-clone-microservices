using Infrastructure.MongoDb;
using Library.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Infrastructure.CollectionSeeders {
    public class LikedPlaylistCollectionSeeder : MongoCollectionSeeder<LikedPlaylist> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<LikedPlaylist> collection, ILogger<IMongoCollection<LikedPlaylist>> logger) {
            var indexBuilder = Builders<LikedPlaylist>.IndexKeys;

            var indexes = new List<CreateIndexModel<LikedPlaylist>> {
                new (indexBuilder.Ascending(nameof(LikedPlaylist.UserId)), new (){ Unique = true }),
                new (indexBuilder.Ascending($"{nameof(LikedPlaylist.Items)}.{nameof(PlaylistItem.Id)}")),
                new (indexBuilder.Ascending($"{nameof(LikedPlaylist.Items)}.{nameof(PlaylistItem.VideoId)}")),
                new (indexBuilder.Ascending($"{nameof(LikedPlaylist.Items)}.{nameof(PlaylistItem.CreateDate)}"))
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
