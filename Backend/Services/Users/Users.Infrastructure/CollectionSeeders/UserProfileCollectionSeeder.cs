using Infrastructure.MongoDb;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Users.Domain.Models;

namespace Users.Infrastructure.CollectionSeeders {
    public class UserProfileCollectionSeeder : MongoCollectionSeeder<UserProfile> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<UserProfile> collection, ILogger<IMongoCollection<UserProfile>> logger) {
            var indexBuilder = Builders<UserProfile>.IndexKeys;

            var indexes = new List<CreateIndexModel<UserProfile>> {
                new (indexBuilder.Ascending(nameof(UserProfile.Handle)),
                    new () { Unique = true, Sparse = true }
                )
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
