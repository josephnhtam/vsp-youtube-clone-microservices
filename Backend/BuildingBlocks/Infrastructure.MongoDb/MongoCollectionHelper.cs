using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using SharedKernel.Exceptions;
using System.Reflection;

namespace Infrastructure.MongoDb {
    public static class MongoCollectionHelper {

        private static AsyncRetryPolicy retryPolicy = Policy
            .Handle<Exception>(ex => ex.Identify(ExceptionCategories.Transient))
            .WaitAndRetryForeverAsync((retryCount) => TimeSpan.FromSeconds(Math.Pow(2, Math.Min(8, retryCount))));

        private class MongoCollectionSeeder { }

        public static async Task SeedMongoCollections (this IHost app, params Assembly[] assemblies) {
            assemblies = assemblies.Union(new List<Assembly> { typeof(MongoCollectionHelper).Assembly }).ToArray();

            foreach (var assembly in assemblies) {
                foreach (var seederType in assembly.GetExportedTypes()) {
                    if (seederType.IsClass && !seederType.IsAbstract) {
                        var documentType = GetDocumentType(seederType);

                        if (documentType != null) {
                            using var scope = app.Services.CreateScope();
                            var services = scope.ServiceProvider;

                            var logger = services.GetRequiredService<ILogger<MongoCollectionSeeder>>();

                            try {
                                var collectionType = typeof(IMongoCollection<>).MakeGenericType(documentType);
                                var loggerType = typeof(ILogger<>).MakeGenericType(collectionType);

                                var collection = services.GetService(collectionType);

                                if (collection != null) {
                                    var collectionLogger = services.GetRequiredService(loggerType);

                                    var seederInstance = Activator.CreateInstance(seederType);
                                    var seedMethod = seederType.GetMethod("SeedAsync", BindingFlags.Instance | BindingFlags.Public);

                                    await retryPolicy.ExecuteAsync(async () => {
                                        try {
                                            var seedTask = seedMethod!.Invoke(seederInstance, new object[] { services, collection, collectionLogger }) as Task;
                                            await seedTask!;
                                        } catch (Exception ex) {
                                            logger.LogError(ex, "Failed to seed the collection of {DocumentType}", documentType.Name);
                                            throw;
                                        }
                                    });
                                }
                            } catch (Exception ex) {
                                logger.LogError(ex, "Failed to seed the collection of {DocumentType}", documentType.Name);
                            }
                        }
                    }
                }
            }
        }

        public static async Task SeedMongoCollection<TDocument> (this IHost app, Func<IMongoCollection<TDocument>, IServiceProvider, ILogger<IMongoCollection<TDocument>>, Task> seedingTask) {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<IMongoCollection<TDocument>>();
            var logger = services.GetRequiredService<ILogger<IMongoCollection<TDocument>>>();

            await retryPolicy.ExecuteAsync(async () => {
                try {
                    logger.LogInformation("Trying to seed {Collection} Collection", nameof(IMongoCollection<TDocument>));
                    await seedingTask.Invoke(context, services, logger);
                    logger.LogInformation("{Collection} Collection seeding succeeds", nameof(IMongoCollection<TDocument>));
                } catch (Exception ex) {
                    logger.LogError(ex, "{Collection} Collection seeding failed", nameof(IMongoCollection<TDocument>));
                    throw;
                }
            });
        }

        private static Type? GetDocumentType (Type seederType) {
            while (!seederType.IsGenericType || seederType.GetGenericTypeDefinition() != typeof(MongoCollectionSeeder<>)) {
                if (seederType.BaseType != null) {
                    seederType = seederType.BaseType;
                } else {
                    return null;
                }
            }

            return seederType.GetGenericArguments()[0];
        }

    }
}
