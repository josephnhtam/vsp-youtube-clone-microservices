using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.MongoDb {
    public abstract class MongoCollectionSeeder<TDocument> {
        public abstract Task SeedAsync (IServiceProvider services, IMongoCollection<TDocument> collection, ILogger<IMongoCollection<TDocument>> logger);
    }
}
