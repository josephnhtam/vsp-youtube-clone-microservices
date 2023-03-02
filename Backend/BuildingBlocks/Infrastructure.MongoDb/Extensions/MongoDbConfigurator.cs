using Infrastructure.MongoDb.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.Extensions {
    public class MongoDbConfigurator : IMongoDbConfigurator {
        public MongoClientSettings? ClientSettings { get; private set; }
        public Action<MongoDbContextConfiguration>? DbContextConfiguration { get; private set; }
        public HealthCheckConfiguration? HealthCheckConfiguration { get; private set; }

        public IMongoDbConfigurator ConfigureMongoClientSettings (MongoClientSettings clientSettings) {
            ClientSettings = clientSettings;
            return this;
        }

        public IMongoDbConfigurator ConfigureDbContext (Action<MongoDbContextConfiguration> configure) {
            DbContextConfiguration = configure;
            return this;
        }

        public IMongoDbConfigurator AddHealthCheck (string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null) {
            HealthCheckConfiguration = new HealthCheckConfiguration(name, failureStatus, tags, timeout);
            return this;
        }
    }

    public record HealthCheckConfiguration (string? Name = null, HealthStatus? FailureStatus = null, IEnumerable<string>? Tags = null, TimeSpan? Timeout = null);
}
