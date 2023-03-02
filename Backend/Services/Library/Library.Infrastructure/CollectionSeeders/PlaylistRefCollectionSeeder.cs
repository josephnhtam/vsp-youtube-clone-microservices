using Infrastructure.MongoDb;
using Library.Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Infrastructure.CollectionSeeders {
    public class PlaylistRefCollectionSeeder : MongoCollectionSeeder<PlaylistRef> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<PlaylistRef> collection, ILogger<IMongoCollection<PlaylistRef>> logger) {
            var indexBuilder = Builders<PlaylistRef>.IndexKeys;

            var indexes = new List<CreateIndexModel<PlaylistRef>> {
                new (indexBuilder.Ascending(nameof(PlaylistRef.UserId))),

                new (indexBuilder.Ascending(nameof(PlaylistRef.PlaylistId))),

                new (indexBuilder
                    .Ascending(nameof(PlaylistRef.UserId))
                    .Ascending(nameof(PlaylistRef.PlaylistId)),
                    new (){
                        Unique = true
                    }
                ),

                new (indexBuilder.Ascending(nameof(PlaylistRef.CreateDate))),
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
