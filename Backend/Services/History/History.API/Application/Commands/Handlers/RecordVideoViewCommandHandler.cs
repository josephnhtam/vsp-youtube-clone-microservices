using Application.Handlers;
using Domain.Contracts;
using History.API.Application.Configurations;
using History.Domain.Contracts;
using History.Infrastructure.Contracts;
using MediatR;
using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;
using StackExchange.Redis;
using System.Text.Json;

namespace History.API.Application.Commands.Handlers {
    public class RecordVideoViewCommandHandler : ICommandHandler<RecordVideoViewCommand> {

        private readonly IDatabase _redisDb;
        private readonly IVideoRepository _videoRepository;
        private readonly IUserProfileRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserHistoryCommandManager _userHistoryCommandManager;
        private readonly MetricsSyncConfiguration _config;
        private readonly ILogger<RecordVideoViewCommandHandler> _logger;

        public RecordVideoViewCommandHandler (
            IConnectionMultiplexer redisConn,
            IVideoRepository videoRepository,
            IUserProfileRepository userRepository,
            IUnitOfWork unitOfWork,
            IUserHistoryCommandManager userHistoryCommandManager,
            IOptions<MetricsSyncConfiguration> config,
            ILogger<RecordVideoViewCommandHandler> logger) {
            _redisDb = redisConn.GetDatabase();
            _videoRepository = videoRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _userHistoryCommandManager = userHistoryCommandManager;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<Unit> Handle (RecordVideoViewCommand request, CancellationToken cancellationToken) {
            var video = await GetVideoAsync(request, cancellationToken);

            if (video != null) {
                await _redisDb.StringIncrementAsync($"video_views#{video.Id}", 1);

                if (request.UserId != null) {
                    var userProfile = await _userRepository.GetUserProfileAsync(request.UserId, false, cancellationToken);

                    if (userProfile != null && userProfile.RecordWatchHistory) {
                        await _userHistoryCommandManager.AddUserWatchHistory(
                            request.UserId,
                            video.Id,
                            video.Title,
                            video.Tags,
                            video.LengthSeconds,
                            DateTimeOffset.UtcNow,
                            cancellationToken);
                    }
                }
            }

            return Unit.Value;
        }

        private async Task<VideoCache?> GetVideoAsync (RecordVideoViewCommand request, CancellationToken cancellationToken) {
            VideoCache? videoCache = null;

            var key = $"video#{request.VideoId}";

            try {
                var cachedVideoData = await _redisDb.StringGetAsync(key);

                if (cachedVideoData.HasValue) {
                    videoCache = JsonSerializer.Deserialize<VideoCache>(cachedVideoData.ToString());
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to deserialize video ({VideoId}) from redis cache", request.VideoId);
                videoCache = null;
            }

            if (videoCache == null) {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId, false, cancellationToken);

                if (video != null) {
                    if (video.Status != Domain.Models.VideoStatus.Published) {
                        return null;
                    }

                    if (video.Metrics.NextSyncDate == null) {
                        video.SetViewsMetricsSyncDate(DateTimeOffset.UtcNow + _config.SyncDelay);
                        await _unitOfWork.CommitAsync(cancellationToken);
                    }

                    videoCache = new VideoCache {
                        Id = video.Id,
                        Title = video.Title,
                        Tags = video.Tags.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray(),
                        LengthSeconds = video.LengthSeconds ?? 0,
                        NextSyncDate = video.Metrics.NextSyncDate
                    };

                    var timeToLive = videoCache.NextSyncDate - DateTimeOffset.UtcNow;
                    if (timeToLive.HasValue && timeToLive.Value.TotalSeconds > 0) {
                        await _redisDb.StringSetAsync(key, JsonSerializer.Serialize(videoCache), timeToLive);
                    } else if (_config.SyncDelaySeconds > 0) {
                        await _redisDb.StringSetAsync(key, JsonSerializer.Serialize(videoCache),
                            TimeSpan.FromSeconds(Math.Min(_config.SyncDelaySeconds, 30)));
                    }
                }
            }

            if (videoCache == null) {
                throw new AppException($"Video {request.VideoId} not found", null, StatusCodes.Status404NotFound);
            }

            return videoCache;
        }

        private class VideoCache {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string[] Tags { get; set; }
            public int LengthSeconds { get; set; }
            public DateTimeOffset? NextSyncDate { get; set; }
        }
    }
}
