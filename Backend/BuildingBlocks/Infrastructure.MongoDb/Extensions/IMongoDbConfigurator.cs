using Infrastructure.MongoDb.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.Extensions {
    public interface IMongoDbConfigurator {
        IMongoDbConfigurator ConfigureMongoClientSettings (MongoClientSettings clientSettings);
        IMongoDbConfigurator ConfigureDbContext (Action<MongoDbContextConfiguration> configure);
        IMongoDbConfigurator AddHealthCheck (string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null);
    }
}