using History.Domain.Models;
using Infrastructure.MongoDb;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace History.Infrastructure.CollectionSeeders {
    public class UserProfileCollectionSeeder : MongoCollectionSeeder<UserProfile> {
        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<UserProfile> collection, ILogger<IMongoCollection<UserProfile>> logger) {
            //var indexBuilder = Builders<UserProfile>.IndexKeys;

            //var indexes = new List<CreateIndexModel<UserProfile>> {
            //    new (indexBuilder.Ascending(nameof(UserProfile.DisplayName)))
            //};

            //await collection.Indexes.CreateManyAsync(indexes);
        }
    }
}
