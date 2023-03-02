using Elasticsearch.Net;

namespace Infrastructure.Elasticsearch.Extensions {
    public static class ExceptionExtensions {

        public static bool IsTransientFailure (this ElasticsearchClientException ex) {
            return ex.FailureReason == PipelineFailure.MaxRetriesReached ||
                   ex.FailureReason == PipelineFailure.MaxTimeoutReached;
        }

    }
}
