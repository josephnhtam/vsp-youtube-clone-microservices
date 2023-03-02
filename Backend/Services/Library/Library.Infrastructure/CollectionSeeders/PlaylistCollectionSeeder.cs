using Infrastructure.MongoDb;
using Library.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Infrastructure.CollectionSeeders {
    public class PlaylistCollectionSeeder : MongoCollectionSeeder<Playlist> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<Playlist> collection, ILogger<IMongoCollection<Playlist>> logger) {
            var indexBuilder = Builders<Playlist>.IndexKeys;

            var indexes = new List<CreateIndexModel<Playlist>> {
                new (indexBuilder.Ascending(nameof(Playlist.UserId))),
                new (indexBuilder.Ascending(nameof(Playlist.UpdateDate))),
                new (indexBuilder.Ascending($"{nameof(Playlist.Items)}.{nameof(PlaylistItem.Id)}")),
                new (indexBuilder.Ascending($"{nameof(Playlist.Items)}.{nameof(PlaylistItem.VideoId)}")),
                new (indexBuilder.Ascending($"{nameof(Playlist.Items)}.{nameof(OrderedPlaylistItem.Position)}")),
                new (indexBuilder.Ascending($"{nameof(Playlist.Items)}.{nameof(PlaylistItem.CreateDate)}"))
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
