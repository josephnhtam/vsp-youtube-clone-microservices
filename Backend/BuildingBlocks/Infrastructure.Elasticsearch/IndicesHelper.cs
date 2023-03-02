using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using Polly;
using Polly.Retry;
using SharedKernel.Exceptions;
using System.Reflection;

namespace Infrastructure.Elasticsearch {
    public static class IndicesHelper {

        private static AsyncRetryPolicy retryPolicy = RetryPolicy
            .Handle<Exception>(ex => ex.Identify(ExceptionCategories.Transient))
            .WaitAndRetryForeverAsync((retryCount) => TimeSpan.FromSeconds(Math.Pow(2, Math.Min(8, retryCount))));

        public static async Task CreateElasticsearchIndices (this IHost app, params Assembly[] assemblies) {
            using var scope = app.Services.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<IElasticClient>();

            IEnumerable<IIndexCreator> indexCreators = assemblies
                .SelectMany(x => x.GetExportedTypes())
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => typeof(IIndexCreator).IsAssignableFrom(x))
                .Select(x => (Activator.CreateInstance(x) as IIndexCreator)!)
                .Where(x => x != null);

            foreach (var indexCreator in indexCreators) {
                var indexName = indexCreator.GetIndexName();

                var logger = (scope.ServiceProvider.GetRequiredService(typeof(ILogger<>).MakeGenericType(indexCreator.GetType())) as ILogger)!;

                await retryPolicy.ExecuteAsync(async () => {
                    try {
                        logger.LogInformation("Trying to creating Index ({IndexName})", indexName);

                        if ((await client.Indices.ExistsAsync(indexName)).Exists) {
                            logger.LogInformation("Index ({IndexName}) already exists", indexName);
                        } else {
                            await client.Indices.CreateAsync(indexName, indexCreator.CreateIndexRequest);
                            logger.LogInformation("Index ({IndexName}) is created", indexName);
                        }
                    } catch (Exception ex) {
                        logger.LogError(ex, "Failed to create index ({IndexName})", indexName);
                        throw;
                    }
                });
            }
        }

    }
}
