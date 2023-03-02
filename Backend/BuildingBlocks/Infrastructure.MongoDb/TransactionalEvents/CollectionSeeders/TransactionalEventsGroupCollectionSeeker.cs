using Infrastructure.MongoDb.TransactionalEvents.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.TransactionalEvents.CollectionSeeders {
    public class TransactionalEventsGroupCollectionSeeker : MongoCollectionSeeder<TransactionalEventsGroup> {

        public override async Task SeedAsync (IServiceProvider services, IMongoCollection<TransactionalEventsGroup> collection, ILogger<IMongoCollection<TransactionalEventsGroup>> logger) {
            var indexBuilder = Builders<TransactionalEventsGroup>.IndexKeys;

            var indexes = new List<CreateIndexModel<TransactionalEventsGroup>> {
                new (indexBuilder.Ascending(x => x.AvailableDate)),
                new (indexBuilder.Ascending("__selection_token")),
            };

            await collection.Indexes.CreateManyAsync(indexes);
        }

    }
}
