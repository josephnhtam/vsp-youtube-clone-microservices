using Domain.Contracts;
using Domain.TransactionalEvents.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using Prometheus;
using SharedKernel.Exceptions;
using SharedKernel.Processors;
using System.Diagnostics;
using VideoProcessor.Application.BackgroundTasks.Processors.FileDownloaders;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoInfoGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoPreviewThumbnailGenerators;
using VideoProcessor.Application.BackgroundTasks.Processors.VideoThumbnailGenerators;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.Infrastructure;
using VideoProcessor.Domain.Contracts;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks {
    public partial class VideoProcessingService : BackgroundService {

        private static readonly ActivitySource _activitySource = new ActivitySource("VideoProcessor");

        private readonly Gauge _concurrentVideoProcessingTasksGauge = Metrics.CreateGauge(
            "concurrent_video_processing_tasks",
            "Number of concurrent video processing tasks.");

        private readonly Gauge _concurrentVideoProcessingTasksPercentageGauge = Metrics.CreateGauge(
            "concurrent_video_processing_tasks_percentage",
            "100 * Number of concurrent video processing tasks / Max number of concurrent video processing tasks.");

        private readonly IServiceProvider _services;
        private readonly VideoProcessorConfiguration _config;
        private readonly ILogger<VideoProcessingService> _logger;
        private readonly RateLimitedRequestProcessor _requestProcessor;

        private int _concurrentProcesses = 0;

        public VideoProcessingService (
            IServiceProvider services,
            IOptions<VideoProcessorConfiguration> config,
            ILogger<VideoProcessingService> logger
            ) {
            _services = services;
            _config = config.Value;
            _logger = logger;
            _requestProcessor = new RateLimitedRequestProcessor(new RateLimitedRequestProcessorOptions {
                MaxConcurrentProcessingLimit = _config.MaxConcurrentProcessingLimit,
                MaxProcessingRateLimit = _config.MaxProcessingRateLimit
            }, logger);
        }

        protected override async Task ExecuteAsync (CancellationToken stoppingToken) {
            await RemoveAllTempDirectoryAsync(_services);
            await _requestProcessor.RunAsync(async () => await ProcessVideoAsync(stoppingToken), stoppingToken);
        }

        public override async Task StopAsync (CancellationToken cancellationToken) {
            _logger.LogInformation("Video processing service is stopping");
            await base.StopAsync(cancellationToken);

            _logger.LogInformation("Removing temp directories");
            await RemoveAllTempDirectoryAsync(_services);

            _logger.LogInformation("Video processing service is stopped");
        }

        private async Task ProcessVideoAsync (CancellationToken stoppingToken) {
            using var scope = _services.CreateScope();
            var services = scope.ServiceProvider;

            IReadOnlyVideo? video = null;

            try {
                video = await PollVideoAsync(services, stoppingToken);
            } catch (OperationCanceledException) {
                _logger.LogWarning("Video processing is cancelled");
                return;
            }

            if (video != null) {
                await DoProcessVideoAsync(services, video /*, stoppingToken */);
            }
        }

        private async Task DoProcessVideoAsync (IServiceProvider services, IReadOnlyVideo video, CancellationToken cancellationToken = default) {
            Guid tempFilesId = Guid.NewGuid();

            try {
                using var processingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var processingCancellationToken = processingCts.Token;

                await using (new VideoProcessingLock(services, video.Id, processingCts, cancellationToken)) {
                    await using (var metricsUpdater = new ConcurrentProcessesMetricsUpdater(this)) {
                        await metricsUpdater.Execute();

                        using (CreateVideoProcessingActivity(video)) {
                            try {
                                string tempDirPath = await GetTempDirectoryAsync(services, tempFilesId);
                                string videoFilePath = await DownloadVideoAsync(services, video, tempDirPath, processingCancellationToken);

                                VideoInfo videoInfo = await GenerateVideoInfoAsync(services, video, videoFilePath, processingCancellationToken);
                                video = await SetVideoInfo(services, video, videoInfo);

                                if (video.Status == VideoProcessingStatus.ProcessingThumbnails) {
                                    video = await ProcessThumbnailsAsync(services, video, tempDirPath, videoFilePath, videoInfo, processingCancellationToken);
                                }

                                if (video.Status == VideoProcessingStatus.ProcessingVideos) {
                                    video = await ProcessVideosAsync(services, video, tempDirPath, videoFilePath, videoInfo, processingCancellationToken);
                                }
                            } catch (OperationCanceledException) {
                                _logger.LogWarning("Processing of video ({VideoId}) is canceled.", video.Id);
                                return;
                            } catch (Exception ex) when (IsRetryableException(ex)) {
                                _logger.LogWarning(ex, "Processing of video ({VideoId}) is failed due to a transient exception.", video.Id);
                                await RetryVideoOrFailVideoProcessing(services, video);
                            } catch (Exception ex) when (ex is not ConflictException && ex is not DbUpdateConcurrencyException) {
                                _logger.LogError(ex, "Processing of video ({VideoId}) is fatally failed.", video.Id);
                                await SetVideoProcessingFailed(services, video);
                            }
                        }
                    }
                };
            } catch (Exception ex) when (ex is ConflictException || ex is DbUpdateConcurrencyException) {
                _logger.LogWarning(
                    $"More than one processor instance trying to process the video ({video.Id}) concurrently\n" +
                    "which may occur if the time is not synchronized or lock duration too small.");
            } finally {
                await CleanUpTempFiles(services, tempFilesId);
            }
        }

        private bool IsRetryableException (Exception ex) {
            return ex.Identify(ExceptionCategories.Transient) || ex is IOException;
        }

        private Activity? CreateVideoProcessingActivity (IReadOnlyVideo video) {
            var activity = _activitySource.StartActivity("ProcessVideo");
            activity?.SetTag("CreatorId", video.CreatorId);
            activity?.SetTag("OriginalFileName", video.OriginalFileName);
            activity?.SetTag("AvailableDate", video.AvailableDate.ToString());
            activity?.SetTag("RetryCount", video.RetryCount);
            return activity;
        }

        private async Task<IReadOnlyVideo> ProcessVideosAsync (IServiceProvider services, IReadOnlyVideo video, string tempDirPath, string videoFilePath, VideoInfo videoInfo, CancellationToken cancellationToken) {
            var processingSteps = video.ProcessingSteps.OrderBy(x => x.Height);

            foreach (var processingStep in processingSteps) {
                bool isRequired = video.Videos.Count == 0;

                if (!processingStep.Complete) {
                    using (var activity = _activitySource.StartActivity("GenerateVideo")) {
                        activity?.SetTag("Resolution", processingStep.Height);

                        var processedVideo = await GenerateVideoAsync(
                            services, video, videoInfo,
                            videoFilePath, processingStep, isRequired,
                            tempDirPath, cancellationToken);

                        if (processedVideo != null) {
                            video = await AddVideoAsync(services, video, processingStep.Label, processedVideo!);
                        }
                    }
                }
            }

            if (video.Videos.Count > 0) {
                return await SetVideoProcessedAsync(services, video);
            } else {
                throw new Exception("No video is processed");
            }
        }

        private async Task<IReadOnlyVideo> ProcessThumbnailsAsync (IServiceProvider services, IReadOnlyVideo video, string tempDirPath, string videoFilePath, VideoInfo videoInfo, CancellationToken cancellationToken) {
            List<VideoThumbnail> thumbnails;
            VideoPreviewThumbnail previewThumbnail;

            using (_activitySource.StartActivity("GenerateVideoThumbnail")) {
                thumbnails = await GenerateVideoThumbnail(
                services, video, videoInfo,
                videoFilePath, tempDirPath,
                cancellationToken);
            }

            using (_activitySource.StartActivity("GenerateVideoPreviewThumbnail")) {
                previewThumbnail = await GenerateVideoPreviewThumbnail(
                services, video, videoInfo,
                videoFilePath, tempDirPath,
                cancellationToken);
            }

            return await AddVideoThumbnailsAsync(services, video, thumbnails, previewThumbnail);
        }

        private async Task RemoveAllTempDirectoryAsync (IServiceProvider services) {
            using var scope = services.CreateScope();
            var tmpDirRepository = scope.ServiceProvider.GetRequiredService<ITempDirectoryRepository>();
            await tmpDirRepository.RemoveAllTempDirectoriesAsync();
        }

        private async Task<string> GetTempDirectoryAsync (IServiceProvider services, Guid tempFilesId) {
            var tmpDirRepository = services.GetRequiredService<ITempDirectoryRepository>();
            return await tmpDirRepository.GetTempDirectoryAsync(tempFilesId);
        }

        private async Task<IReadOnlyVideo> SetVideoProcessedAsync (IServiceProvider services, IReadOnlyVideo video) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            return await ExecuteTransactionalVideoUpdateAsync(services, video, async (video) => {
                video.SetProcessed();

                await unitOfWork.CommitAsync();

                _logger.LogInformation("Processing of video ({VideoId}) is complete", video.Id);
            });
        }

        private async Task<IReadOnlyVideo> AddVideoAsync (IServiceProvider services, IReadOnlyVideo video, string processingStepLabel, ProcessedVideo processedVideo) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            return await ExecuteTransactionalVideoUpdateAsync(services, video, async (video) => {
                video.ProcessingSteps.FirstOrDefault(x => x.Label == processingStepLabel)?.SetComplete();
                video.AddVideo(processedVideo);

                await unitOfWork.CommitAsync();

                _logger.LogInformation("Processed video ({ProcessedVideoFileId}) is added to video ({VideoId})", processedVideo.VideoFileId, video.Id);
            });
        }

        private async Task<IReadOnlyVideo> AddVideoThumbnailsAsync (IServiceProvider services, IReadOnlyVideo video, List<VideoThumbnail> thumbnails, VideoPreviewThumbnail? previewThumbnail) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            return await ExecuteTransactionalVideoUpdateAsync(services, video, async (video) => {
                video.AddThumbnails(thumbnails, previewThumbnail);

                await unitOfWork.CommitAsync();
            });
        }

        private async Task<VideoInfo> GenerateVideoInfoAsync (IServiceProvider services, IReadOnlyVideo video, string videoFilePath, CancellationToken cancellationToken) {
            using (_activitySource.StartActivity("GenerateVideoInfo")) {
                using var scope = services.CreateScope();
                _logger.LogInformation("Generating video info ({VideoId})", video.Id);
                var generator = scope.ServiceProvider.GetRequiredService<IVideoInfoGenerator>();
                var result = await generator.GenerateAsync(video, videoFilePath, cancellationToken);
                _logger.LogInformation("Video info ({VideoId}) is generated", video.Id);
                return result;
            }
        }

        private async Task<List<VideoThumbnail>> GenerateVideoThumbnail (IServiceProvider services, IReadOnlyVideo video, VideoInfo videoInfo, string videoFilePath, string tempDirPath, CancellationToken cancellationToken) {
            using var scope = services.CreateScope();
            _logger.LogInformation("Generating video thumbnail ({VideoId})", video.Id);
            var generator = scope.ServiceProvider.GetRequiredService<IVideoThumbnailGenerator>();
            var result = await generator.GenerateAsync(video, videoInfo, videoFilePath, tempDirPath, cancellationToken);
            _logger.LogInformation("Video thumbnail ({VideoId}) is generated", video.Id);
            return result;
        }

        private async Task<VideoPreviewThumbnail> GenerateVideoPreviewThumbnail (IServiceProvider services, IReadOnlyVideo video, VideoInfo videoInfo, string videoFilePath, string tempDirPath, CancellationToken cancellationToken) {
            using var scope = services.CreateScope();
            _logger.LogInformation("Generating video preview thumbnail ({VideoId})", video.Id);
            var generator = scope.ServiceProvider.GetRequiredService<IVideoPreviewThumbnailGenerator>();
            var result = await generator.GenerateAsync(video, videoInfo, videoFilePath, tempDirPath, cancellationToken);
            _logger.LogInformation("Video preview thumbnail ({VideoId}) is generated", video.Id);
            return result;
        }

        private async Task<ProcessedVideo?> GenerateVideoAsync (IServiceProvider services, IReadOnlyVideo video, VideoInfo videoInfo, string videoFilePath, VideoProcessingStep processingStep, bool required, string tempDirPath, CancellationToken cancellationToken) {
            using var scope = services.CreateScope();
            _logger.LogInformation("Generating video ({VideoId}) of resoultion ({Resoltuion})", video.Id, processingStep.Height);
            var generator = scope.ServiceProvider.GetRequiredService<IVideoGenerator>();
            var result = await generator.GenerateAsync(video, videoInfo, videoFilePath, processingStep, required, tempDirPath, cancellationToken);
            _logger.LogInformation("Video ({VideoId}) of resoultion ({Resoltuion}) is generated", video.Id, processingStep.Height);
            return result;
        }

        private async Task<string> DownloadVideoAsync (IServiceProvider services, IReadOnlyVideo video, string tempDirPath, CancellationToken cancellationToken) {
            using (_activitySource.StartActivity("DownloadVideo")) {
                using var scope = services.CreateScope();
                var downloader = scope.ServiceProvider.GetRequiredService<IFileDownloader>();

                try {
                    _logger.LogInformation(@"Downloading video ({VideoId}) to ""{TempDirPath}"" for processing", video.Id, tempDirPath);
                    var videoPath = await downloader.DownloadVideoAsync(video, tempDirPath, cancellationToken);
                    _logger.LogInformation(@"Video ({VideoId}) is downloaded to ""{VideoPath}"" for processing", video.Id, videoPath);
                    return videoPath;
                } catch (Exception ex) {
                    _logger.LogError(ex, @"Failed to download video ({VideoId}) to ""{TempDirPath}"" for processing", video.Id, tempDirPath);
                    throw;
                }
            }
        }

        private async Task<IReadOnlyVideo> SetVideoInfo (IServiceProvider services, IReadOnlyVideo video, VideoInfo videoInfo) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            return await ExecuteTransactionalVideoUpdateAsync(services, video, async (video) => {
                video.SetVideoInfo(videoInfo);

                await unitOfWork.CommitAsync();

                _logger.LogInformation("Video info ({VideoId}) is set", video.Id);
            });
        }

        private async Task<IReadOnlyVideo> SetVideoProcessingFailed (IServiceProvider services, IReadOnlyVideo video) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            return await ExecuteTransactionalVideoUpdateAsync(services, video, async (video) => {
                video.SetFailed();

                await unitOfWork.CommitAsync();

                _logger.LogInformation("Processing of video ({VideoId}) is failed", video.Id);
            });
        }

        private async Task CleanUpTempFiles (IServiceProvider services, Guid tempFilesId) {
            try {
                var tmpDirRepository = services.GetRequiredService<ITempDirectoryRepository>();
                await tmpDirRepository.RemoveTempDirectoryAsync(tempFilesId);

                _logger.LogInformation("Temp files ({VideoId}) are cleaned up successfully", tempFilesId);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to clean up temp files");
            }
        }

        private async Task<IReadOnlyVideo> RetryVideoOrFailVideoProcessing (IServiceProvider services, IReadOnlyVideo video, bool considerMaxRetryCount = true) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            return await ExecuteTransactionalVideoUpdateAsync(services, video, async (video) => {
                if (considerMaxRetryCount && video.RetryCount >= _config.MaxRetryCount) {
                    video.SetFailed();

                    await unitOfWork.CommitAsync();

                    _logger.LogWarning("Processing of video ({VideoId}) exceeds max retry count ({MaxRetryCount})", video.Id, _config.MaxRetryCount);
                } else {
                    video.RetryLater(TimeSpan.FromSeconds(_config.RetryDelaySeconds));
                    await unitOfWork.CommitAsync();

                    _logger.LogInformation("Processing of video ({VideoId}) will be retried after {RetryDelaySeconds} seconds", video.Id, _config.RetryDelaySeconds);
                }
            });
        }

        private async Task<Video?> PollVideoAsync (IServiceProvider services, CancellationToken cancellationToken) {
            using var suppressScope = SuppressInstrumentationScope.Begin();

            Video? result = null;

            var repository = services.GetRequiredService<IVideoRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await unitOfWork.ExecuteTransactionAsync(async () => {
                var video = (await repository.GetVideosToProcessAsync(1, cancellationToken)).FirstOrDefault();

                if (video != null) {
                    result = video;

                    if (video.Status == VideoProcessingStatus.Pending) {
                        video.SetProcessingStarted();
                    }

                    video.IncrementLockVersion();
                    video.PostponeAvailableDate(TimeSpan.FromSeconds(_config.ProcessingLockDurationSeconds));

                    await unitOfWork.CommitAsync(cancellationToken);
                }
            }, null, cancellationToken);

            return result;
        }

        private async Task<IReadOnlyVideo> ExecuteTransactionalVideoUpdateAsync (IServiceProvider services, IReadOnlyVideo video, Func<Video, Task> task) {
            var repository = services.GetRequiredService<IVideoRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();
            var transactionalEventsContext = services.GetRequiredService<ITransactionalEventsContext>();

            Video? dbVideo = null;

            try {
                await unitOfWork.ExecuteTransactionAsync(async () => {
                    dbVideo = await repository.GetVideoByIdAsync(video.Id);

                    if (dbVideo == null || dbVideo.LockVersion != video.LockVersion) {
                        throw new ConflictException();
                    }

                    await task.Invoke(dbVideo);
                });
            } catch (Exception ex) {
                _logger.LogError(ex, "An error occurred during executing task with lock version check for video ({VideoId})", video.Id);
                throw;
            }

            // The ordering of integration events publishing is not neccessary
            transactionalEventsContext.ResetDefaultEventsGroudId();

            return dbVideo!;
        }

    }
}
