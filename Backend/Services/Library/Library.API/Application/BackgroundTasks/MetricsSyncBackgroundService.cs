using Domain.Contracts;
using EventBus;
using EventBus.RabbitMQ;
using Library.API.Application.Configurations;
using Library.API.Application.IntegrationEvents;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using SharedKernel.Processors;

namespace Library.API.Application.BackgroundTasks {
    public class MetricsSyncBackgroundService : BackgroundService {

        private readonly IServiceProvider _services;
        private readonly RateLimitedRequestProcessor _requestProcessor;
        private readonly MetricsSyncConfiguration _config;
        private readonly ILogger<MetricsSyncBackgroundService> _logger;

        public MetricsSyncBackgroundService (
            IServiceProvider services,
            IOptions<MetricsSyncConfiguration> config,
            ILogger<MetricsSyncBackgroundService> logger) {
            _services = services;
            _logger = logger;

            _config = config.Value;
            _requestProcessor = new RateLimitedRequestProcessor(new RateLimitedRequestProcessorOptions {
                MaxConcurrentProcessingLimit = _config.MaxConcurrentProcessingtLimit,
                MaxProcessingRateLimit = _config.MaxProcessingRateLimit
            }, logger);
        }

        protected override async Task ExecuteAsync (CancellationToken stoppingToken) {
            await _requestProcessor.RunAsync(async () => await ProcessMetricsSync(stoppingToken), stoppingToken);
        }

        private async Task ProcessMetricsSync (CancellationToken stoppingToken) {
            using var suppressScope = SuppressInstrumentationScope.Begin();

            using var scope = _services.CreateScope();
            var services = scope.ServiceProvider;

            var videoRepository = services.GetRequiredService<IVideoRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();
            var eventBus = services.GetRequiredService<IEventBus>();

            List<Video>? videos = null;

            videos = await videoRepository.PollVideoForMetricsSyncAsync(_config.FetchCount, stoppingToken);

            if (videos != null && videos.Count > 0) {
                var currentTime = DateTimeOffset.UtcNow;

                var integrationEvents = videos.Select(video =>
                    new VideoVotesMetricsSyncIntegrationEvent(
                        video.Id,
                        video.Metrics.LikesCount,
                        video.Metrics.DislikesCount,
                        currentTime));

                await Task.WhenAll(integrationEvents.Select(ev =>
                    eventBus.PublishEvent(ev, properties => {
                        if (properties is RabbitMQIntegrationEventProperties rmq) {
                            bool isPublic = videos.First(x => x.Id == ev.VideoId).IsPublic;

                            rmq.Headers = new Dictionary<string, object> {
                                { "public", isPublic },
                            };
                        }
                    })
                ));
            }
        }

    }
}
