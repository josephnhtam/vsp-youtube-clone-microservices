using Infrastructure.Idempotency;
using Infrastructure.MongoDb.Configurations;
using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.TransactionalEvents.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.Extensions {
    public class MongoDbBuilder {

        public IServiceCollection Services { get; private set; }

        public MongoDbBuilder (IServiceCollection services) {
            Services = services;
        }

        public MongoDbBuilder AddCollection<TDocument> (string databaseName, string collectionName, MongoCollectionSettings? settings = null) {
            Services.TryAddScoped<IMongoCollection<TDocument>>(services => {
                var client = services.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(databaseName);
                return database.GetCollection<TDocument>(collectionName, settings);
            });

            Services.TryAddScoped<IMongoCollectionContext<TDocument>>(services => {
                var clientContext = services.GetRequiredService<IMongoClientContext>();
                var collection = services.GetRequiredService<IMongoCollection<TDocument>>();
                var config = services.GetRequiredService<IOptions<MongoDbContextConfiguration>>();
                return new MongoCollectionContext<TDocument>(clientContext, collection, config);
            });

            Services.AddScoped<IMongoCollectionContextBase>(services => services.GetRequiredService<IMongoCollectionContext<TDocument>>());

            return this;
        }

        public MongoDbBuilder AddTransactionalEventsCollection (string databaseName, string collectionName = "__transactional_events_group") {
            AddCollection<TransactionalEventsGroup>(databaseName, collectionName);

            return this;
        }

        public MongoDbBuilder AddIdempotentOperationsCollection (string databaseName, string collectionName = "__idempotent_operation") {
            AddCollection<IdempotentOperation>(databaseName, collectionName);

            return this;
        }

    }
}
