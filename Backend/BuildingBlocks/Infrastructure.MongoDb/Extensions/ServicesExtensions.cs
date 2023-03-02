using Infrastructure.MongoDb.Configurations;
using Infrastructure.MongoDb.Contexts;
using Infrastructure.MongoDb.DomainEventsDispatching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.Extensions {
    public static class ServicesExtensions {

        public static MongoDbBuilder AddMongoDb (this IServiceCollection services, Action<IMongoDbConfigurator> configure) {
            var configurator = new MongoDbConfigurator();
            configure.Invoke(configurator);

            if (configurator.ClientSettings == null) {
                throw new InvalidOperationException($"{nameof(configurator.ClientSettings)} cannot be null");
            }

            if (configurator.DbContextConfiguration != null) {
                services.Configure<MongoDbContextConfiguration>(configurator.DbContextConfiguration);
            }

            services.TryAddSingleton<IMongoClient>(new MongoClient(configurator.ClientSettings));

            services.TryAddScoped<IMongoClientContext>(services => {
                var client = services.GetRequiredService<IMongoClient>();
                var options = services.GetRequiredService<IOptions<MongoDbContextConfiguration>>();
                return new MongoClientContext(services, client, options);
            });

            if (configurator.HealthCheckConfiguration != null) {
                services.AddHealthChecks().AddMongoDb(
                    configurator.ClientSettings,
                    configurator.HealthCheckConfiguration.Name,
                    configurator.HealthCheckConfiguration.FailureStatus,
                    configurator.HealthCheckConfiguration.Tags,
                    configurator.HealthCheckConfiguration.Timeout);
            }

            var pack = new ConventionPack() { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElementsByDefault", pack, t => true);

            return new MongoDbBuilder(services);
        }

        public static IServiceCollection AddDomainEventEmittersTracker (this IServiceCollection services) {
            services.TryAddScoped<IDomainEventEmittersTracker, DomainEventEmittersTracker>();

            return services;
        }

    }
}
