using Domain.Contracts;
using EventBus;
using EventBus.RabbitMQ;
using History.API.Application.Configurations;
using History.API.Application.IntegrationEvents;
using History.Domain.Contracts;
using History.Domain.Models;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using SharedKernel.Processors;
using StackExchange.Redis;

namespace History.API.Application.BackgroundTasks {
    public class MetricsSyncBackgroundService : BackgroundService {

        private readonly IServiceProvider _services;
        private readonly IDatabase _redisDb;
        private readonly RateLimitedRequestProcessor _requestProcessor;
        private readonly MetricsSyncConfiguration _config;
        private readonly ILogger<MetricsSyncBackgroundService> _logger;

        public MetricsSyncBackgroundService (
            IServiceProvider services,
            IConnectionMultiplexer redisConn,
            IOptions<MetricsSyncConfiguration> config,
            ILogger<MetricsSyncBackgroundService> logger) {
            _services = services;
            _logger = logger;

            _redisDb = redisConn.GetDatabase();

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

                var viewsCountChanges = videos.Select(v => new {
                    Id = v.Id,
                    ViewsCountChange = _redisDb.StringGetDeleteAsync($"video_views#{v.Id}")
                }).ToDictionary(x => x.Id, x => x.ViewsCountChange);

                await Task.WhenAll(viewsCountChanges.Values);

                foreach (var video in videos) {
                    var task = viewsCountChanges[video.Id];

                    try {
                        var viewsCountChange = (long?)task.Result;

                        if (viewsCountChange.HasValue && viewsCountChange != 0) {
                            videoRepository.Track(video);
                            video.ChangeViewsCount(viewsCountChange.Value);
                        }
                    } catch (Exception ex) {
                        _logger.LogError(ex, "An error has occurred");
                    }
                }

                await unitOfWork.CommitAsync(stoppingToken);

                var integrationEvents = (videos.Select(video =>
                    new VideoViewsMetricsSyncIntegrationEvent(
                        video.Id,
                        video.Metrics.ViewsCount,
                        currentTime)));

                await Task.WhenAll(integrationEvents.Select(ev =>
                    eventBus.PublishEvent(ev, properties => {
                        if (properties is RabbitMQIntegrationEventProperties rmq) {
                            bool isPublic = videos.First(x => x.Id == ev.VideoId).IsPublic;

                            rmq.Headers = new Dictionary<string, object> {
                                { "public", isPublic }
                            };
                        }
                    })
                ));
            }
        }

    }
}
