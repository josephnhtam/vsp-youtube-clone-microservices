using Infrastructure;
using Infrastructure.TransactionalEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry;
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
    public class VideoProcessingService : BackgroundService {

        private static readonly ActivitySource _activitySource = new ActivitySource("VideoProcessor");

        private readonly IServiceProvider _services;
        private readonly VideoProcessorConfiguration _config;
        private readonly ILogger<VideoProcessingService> _logger;
        private readonly RateLimitedRequestProcessor _requestProcessor;

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
            await base.StopAsync(cancellationToken);
            await RemoveAllTempDirectoryAsync(_services);
        }

        private async Task ProcessVideoAsync (CancellationToken cancellationToken) {
            using var scope = _services.CreateScope();
            var services = scope.ServiceProvider;

            var video = await PollVideoAsync(services, cancellationToken);

            if (video != null) {
                Guid tempFilesId = Guid.NewGuid();

                try {
                    using var processingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    var processingCancellationToken = processingCts.Token;

                    await using (new VideoProcessingLock(services, video.Id, processingCts, cancellationToken)) {
                        using (CreateVideoProcessingActivity(video)) {
                            try {
                                string tempDirPath = await GetTempDirectoryAsync(services, tempFilesId);
                                string videoFilePath = await DownloadVideoAsync(services, video, tempDirPath, processingCancellationToken);

                                VideoInfo videoInfo = await GenerateVideoInfoAsync(services, video, videoFilePath, processingCancellationToken);
                                await SetVideoInfo(services, video, videoInfo);

                                if (video.Status == VideoProcessingStatus.ProcessingThumbnails) {
                                    await ProcessThumbnailsAsync(services, video, tempDirPath, videoFilePath, videoInfo, processingCancellationToken);
                                }

                                if (video.Status == VideoProcessingStatus.ProcessingVideos) {
                                    await ProcessVideosAsync(services, video, tempDirPath, videoFilePath, videoInfo, processingCancellationToken);
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
                    };
                } catch (Exception ex) when (ex is ConflictException || ex is DbUpdateConcurrencyException) {
                    _logger.LogWarning(
                        $"More than one processor instance trying to process the video ({video.Id}) concurrently\n" +
                        "which may occur if the time is not synchronized or lock duration too small.");
                } finally {
                    await CleanUpTempFiles(services, tempFilesId);
                }
            }
        }

        private bool IsRetryableException (Exception ex) {
            return ex.Identify(ExceptionCategories.Transient) || ex is IOException;
        }

        private Activity? CreateVideoProcessingActivity (Video video) {
            var activity = _activitySource.StartActivity("ProcessVideo");
            activity?.SetTag("CreatorId", video.CreatorId);
            activity?.SetTag("OriginalFileName", video.OriginalFileName);
            activity?.SetTag("AvailableDate", video.AvailableDate.ToString());
            activity?.SetTag("RetryCount", video.RetryCount);
            return activity;
        }

        private async Task ProcessVideosAsync (IServiceProvider services, Video video, string tempDirPath, string videoFilePath, VideoInfo videoInfo, CancellationToken cancellationToken) {
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
                            await AddVideoAsync(services, video, processingStep, processedVideo!);
                        }
                    }
                }
            }

            if (video.Videos.Count > 0) {
                await SetVideoProcessedAsync(services, video);
            } else {
                throw new Exception("No video is processed");
            }
        }

        private async Task ProcessThumbnailsAsync (IServiceProvider services, Video video, string tempDirPath, string videoFilePath, VideoInfo videoInfo, CancellationToken cancellationToken) {
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

            await AddVideoThumbnailsAsync(services, video, thumbnails, previewThumbnail);
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

        private async Task SetVideoProcessedAsync (IServiceProvider services, Video video) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await ExecuteTransactionalVideoUpdateAsync(services, video, async () => {
                video.SetProcessed();

                await unitOfWork.CommitAsync();

                _logger.LogInformation("Processing of video ({VideoId}) is complete", video.Id);
            });
        }

        private async Task AddVideoAsync (IServiceProvider services, Video video, VideoProcessingStep processingStep, ProcessedVideo processedVideo) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await ExecuteTransactionalVideoUpdateAsync(services, video, async () => {
                processingStep.SetComplete();
                video.AddVideo(processedVideo);

                await unitOfWork.CommitAsync();

                _logger.LogInformation("Processed video ({ProcessedVideoFileId}) is added to video ({VideoId})", processedVideo.VideoFileId, video.Id);
            });
        }

        private async Task AddVideoThumbnailsAsync (IServiceProvider services, Video video, List<VideoThumbnail> thumbnails, VideoPreviewThumbnail? previewThumbnail) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await ExecuteTransactionalVideoUpdateAsync(services, video, async () => {
                video.AddThumbnails(thumbnails, previewThumbnail);

                await unitOfWork.CommitAsync();
            });
        }

        private async Task<VideoInfo> GenerateVideoInfoAsync (IServiceProvider services, Video video, string videoFilePath, CancellationToken cancellationToken) {
            using (_activitySource.StartActivity("GenerateVideoInfo")) {
                using var scope = services.CreateScope();
                _logger.LogInformation("Generating video info ({VideoId})", video.Id);
                var generator = scope.ServiceProvider.GetRequiredService<IVideoInfoGenerator>();
                var result = await generator.GenerateAsync(video, videoFilePath, cancellationToken);
                _logger.LogInformation("Video info ({VideoId}) is generated", video.Id);
                return result;
            }
        }

        private async Task<List<VideoThumbnail>> GenerateVideoThumbnail (IServiceProvider services, Video video, VideoInfo videoInfo, string videoFilePath, string tempDirPath, CancellationToken cancellationToken) {
            using var scope = services.CreateScope();
            _logger.LogInformation("Generating video thumbnail ({VideoId})", video.Id);
            var generator = scope.ServiceProvider.GetRequiredService<IVideoThumbnailGenerator>();
            var result = await generator.GenerateAsync(video, videoInfo, videoFilePath, tempDirPath, cancellationToken);
            _logger.LogInformation("Video thumbnail ({VideoId}) is generated", video.Id);
            return result;
        }

        private async Task<VideoPreviewThumbnail> GenerateVideoPreviewThumbnail (IServiceProvider services, Video video, VideoInfo videoInfo, string videoFilePath, string tempDirPath, CancellationToken cancellationToken) {
            using var scope = services.CreateScope();
            _logger.LogInformation("Generating video preview thumbnail ({VideoId})", video.Id);
            var generator = scope.ServiceProvider.GetRequiredService<IVideoPreviewThumbnailGenerator>();
            var result = await generator.GenerateAsync(video, videoInfo, videoFilePath, tempDirPath, cancellationToken);
            _logger.LogInformation("Video preview thumbnail ({VideoId}) is generated", video.Id);
            return result;
        }

        private async Task<ProcessedVideo?> GenerateVideoAsync (IServiceProvider services, Video video, VideoInfo videoInfo, string videoFilePath, VideoProcessingStep processingStep, bool required, string tempDirPath, CancellationToken cancellationToken) {
            using var scope = services.CreateScope();
            _logger.LogInformation("Generating video ({VideoId}) of resoultion ({Resoltuion})", video.Id, processingStep.Height);
            var generator = scope.ServiceProvider.GetRequiredService<IVideoGenerator>();
            var result = await generator.GenerateAsync(video, videoInfo, videoFilePath, processingStep, required, tempDirPath, cancellationToken);
            _logger.LogInformation("Video ({VideoId}) of resoultion ({Resoltuion}) is generated", video.Id, processingStep.Height);
            return result;
        }

        private async Task<string> DownloadVideoAsync (IServiceProvider services, Video video, string tempDirPath, CancellationToken cancellationToken) {
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

        private async Task SetVideoInfo (IServiceProvider services, Video video, VideoInfo videoInfo) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await ExecuteTransactionalVideoUpdateAsync(services, video, async () => {
                video.SetVideoInfo(videoInfo);

                await unitOfWork.CommitAsync();

                _logger.LogInformation("Video info ({VideoId}) is set", video.Id);
            });
        }

        private async Task SetVideoProcessingFailed (IServiceProvider services, Video video) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await ExecuteTransactionalVideoUpdateAsync(services, video, async () => {
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

        private async Task RetryVideoOrFailVideoProcessing (IServiceProvider services, Video video, bool considerMaxRetryCount = true) {
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await ExecuteTransactionalVideoUpdateAsync(services, video, async () => {
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

        private async Task ExecuteTransactionalVideoUpdateAsync (IServiceProvider services, Video video, Func<Task> task) {
            var repository = services.GetRequiredService<IVideoRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();
            var transactionalEventsContext = services.GetRequiredService<ITransactionalEventsContext>();

            try {
                await unitOfWork.ExecuteTransactionAsync(async () => {
                    var dbVideo = await repository.GetVideoByIdAsync(video.Id);

                    if (dbVideo == null || dbVideo.LockVersion != video.LockVersion) {
                        throw new ConflictException();
                    }

                    await task.Invoke();
                });
            } catch (Exception ex) {
                _logger.LogError(ex, "An error occurred during executing task with lock version check for video ({VideoId})", video.Id);
                throw;
            }

            // The ordering of integration events publishing is not neccessary
            transactionalEventsContext.ResetDefaultEventsGroudId();
        }

        private class VideoProcessingLock : IAsyncDisposable {

            private Task _task;
            private CancellationTokenSource _videoProcessingCts;
            private CancellationTokenSource _completeCts;

            public VideoProcessingLock (IServiceProvider services, Guid videoId, CancellationTokenSource videoProcessingCts, CancellationToken stoppingToken) {
                _videoProcessingCts = videoProcessingCts;
                _completeCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                _task = LockVideo(services, videoId, _completeCts.Token);
            }

            public async ValueTask DisposeAsync () {
                _completeCts.Cancel();
                _completeCts.Dispose();

                try {
                    await _task;
                } catch (OperationCanceledException) { }
            }

            private async Task LockVideo (IServiceProvider services, Guid videoId, CancellationToken stoppingToken) {
                using var scope = services.CreateScope();
                var scopedServices = scope.ServiceProvider;

                var repository = scopedServices.GetRequiredService<IVideoRepository>();
                var unitOfWork = scopedServices.GetRequiredService<IUnitOfWork>();
                var config = scopedServices.GetRequiredService<IOptions<VideoProcessorConfiguration>>().Value;
                var logger = scopedServices.GetRequiredService<ILogger<VideoProcessingLock>>();

                var video = await repository.GetVideoByIdAsync(videoId, stoppingToken);

                if (video == null) {
                    logger.LogError("Video ({VideoId}) not found", videoId);
                    _videoProcessingCts.Cancel();
                    return;
                }

                while (!stoppingToken.IsCancellationRequested) {
                    try {
                        video.PostponeAvailableDate(TimeSpan.FromSeconds(config.ProcessingLockDurationSeconds));
                        await unitOfWork.CommitAsync(stoppingToken);
                    } catch (Exception ex) {
                        logger.LogError(ex, "Failed to postpone video ({VideoId}) processing available date. Cancelling the processing.", videoId);
                        _videoProcessingCts.Cancel();
                        return;
                    }

                    await Task.Delay(
                        (int)Math.Ceiling(config.ProcessingLockDurationSeconds * 1000 * 0.5f),
                        stoppingToken);
                }
            }

        }

        private class ConflictException : Exception {
        }

    }
}
