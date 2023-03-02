using Infrastructure.MongoDb;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Users.Domain.Models;

namespace Users.Infrastructure.CollectionSeeders {
    public class UserChannelCollectionSeeder : MongoCollectionSeeder<UserChannel> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<UserChannel> collection, ILogger<IMongoCollection<UserChannel>> logger) {
            var indexBuilder = Builders<UserChannel>.IndexKeys;

            var indexes = new List<CreateIndexModel<UserChannel>> {
                new (indexBuilder.Ascending(nameof(UserChannel.Handle)),
                    new () { Unique = true, Sparse = true }
                ),
                new (indexBuilder.Ascending($"{nameof(UserChannel.Sections)}.{nameof(ChannelSection.Id)}"))
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
