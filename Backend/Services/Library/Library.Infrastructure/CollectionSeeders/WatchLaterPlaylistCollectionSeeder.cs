using Infrastructure.MongoDb;
using Library.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Infrastructure.CollectionSeeders {
    public class WatchLaterPlaylistCollectionSeeder : MongoCollectionSeeder<WatchLaterPlaylist> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<WatchLaterPlaylist> collection, ILogger<IMongoCollection<WatchLaterPlaylist>> logger) {
            var indexBuilder = Builders<WatchLaterPlaylist>.IndexKeys;

            var indexes = new List<CreateIndexModel<WatchLaterPlaylist>> {
                new (indexBuilder.Ascending(nameof(WatchLaterPlaylist.UserId)), new (){ Unique = true }),
                new (indexBuilder.Ascending($"{nameof(WatchLaterPlaylist.Items)}.{nameof(PlaylistItem.Id)}")),
                new (indexBuilder.Ascending($"{nameof(WatchLaterPlaylist.Items)}.{nameof(PlaylistItem.VideoId)}")),
                new (indexBuilder.Ascending($"{nameof(WatchLaterPlaylist.Items)}.{nameof(OrderedPlaylistItem.Position)}")),
                new (indexBuilder.Ascending($"{nameof(WatchLaterPlaylist.Items)}.{nameof(PlaylistItem.CreateDate)}"))
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
