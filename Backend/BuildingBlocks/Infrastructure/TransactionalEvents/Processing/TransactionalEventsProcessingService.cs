using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Processors;

namespace Infrastructure.TransactionalEvents.Processing {
    public class TransactionalEventsProcessingService : BackgroundService {

        private readonly ITransactionalEventsProcessor _processor;
        private readonly RateLimitedRequestProcessor _requestProcessor;

        public TransactionalEventsProcessingService (
            ITransactionalEventsProcessor? processor,
            IOptions<TransactionalEventsProcessorConfiguration> config,
            ILogger<TransactionalEventsProcessingService> logger) {
            if (processor == null) {
                throw new ArgumentNullException("Transactional events processor is not added");
            }

            _processor = processor;

            var _config = config.Value;
            _requestProcessor = new RateLimitedRequestProcessor(new RateLimitedRequestProcessorOptions {
                MaxConcurrentProcessingLimit = _config.MaxConcurrentProcessingtLimit,
                MaxProcessingRateLimit = _config.MaxProcessingRateLimit,
            }, logger);
        }

        protected override async Task ExecuteAsync (CancellationToken stoppingToken) {
            await _requestProcessor.RunAsync(
                async () => await _processor.ProcessTransactionalEvents(stoppingToken)
                , stoppingToken);
        }

        public override void Dispose () {
            base.Dispose();
            _requestProcessor.Dispose();
        }

    }
}
