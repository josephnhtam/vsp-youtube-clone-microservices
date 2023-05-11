using Domain.Contracts;
using Microsoft.Extensions.Options;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Domain.Contracts;

namespace VideoProcessor.Application.BackgroundTasks {
    public partial class VideoProcessingService {
        private class VideoProcessingLock : IAsyncDisposable {

            private readonly Task _task;
            private readonly CancellationTokenSource _videoProcessingCts;
            private readonly CancellationTokenSource _completeCts;

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
