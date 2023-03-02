using Elasticsearch.Net;
using Infrastructure.Elasticsearch.Extensions;
using Microsoft.AspNetCore.Http;
using Polly;
using SharedKernel.Exceptions;

namespace Infrastructure.Elasticsearch {
    public static class ElasticsearchHelper {

        public static async Task ExecuteAsync (Func<Task> task) {
            var retryPolicy = Polly.Policy
                .Handle<ElasticsearchClientException>(ex => ex.Response.HttpStatusCode == StatusCodes.Status409Conflict)
                .RetryAsync((_, retryCount) => Math.Pow(2, Math.Max(5, retryCount)));

            try {
                await retryPolicy.ExecuteAsync(task);
            } catch (ElasticsearchClientException ex) when (ex.IsTransientFailure()) {
                throw new TransientException();
            }
        }

    }
}
