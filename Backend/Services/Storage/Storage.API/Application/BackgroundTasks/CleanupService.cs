using Domain.Contracts;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using SharedKernel.Processors;
using Storage.API.Application.Configurations;
using Storage.Domain.Contracts;
using Storage.Domain.Models;

namespace Storage.API.Application.BackgroundTasks {
    public class CleanupService : BackgroundService {

        private readonly IServiceProvider _services;
        private readonly CleanupConfiguration _config;
        private readonly ILogger<CleanupService> _logger;
        private readonly RateLimitedRequestProcessor _requestProcessor;

        public CleanupService (
            IServiceProvider services,
            IOptions<CleanupConfiguration> config,
            ILogger<CleanupService> logger) {
            _services = services;
            _config = config.Value;
            _logger = logger;

            _requestProcessor = new RateLimitedRequestProcessor(new RateLimitedRequestProcessorOptions {
                MaxConcurrentProcessingLimit = _config.MaxConcurrentProcessingtLimit,
                MaxProcessingRateLimit = _config.MaxProcessingRateLimit
            }, logger);
        }

        protected override async Task ExecuteAsync (CancellationToken stoppingToken) {
            await _requestProcessor.RunAsync(async () => await ProcessCleanupAsync(stoppingToken), stoppingToken);
        }

        private async Task ProcessCleanupAsync (CancellationToken cancellationToken) {
            using var suppressScope = SuppressInstrumentationScope.Begin();

            using var scope = _services.CreateScope();
            var services = scope.ServiceProvider;

            var fileTrackingRepository = services.GetRequiredService<IFileTrackingRepository>();
            var fileRepositoy = services.GetRequiredService<IFileRepository>();
            var storageRepository = services.GetRequiredService<IStorageRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            await RemoveUnusedFilesAsync(fileTrackingRepository, fileRepositoy, storageRepository, unitOfWork, cancellationToken);
        }

        private async Task RemoveUnusedFilesAsync (IFileTrackingRepository fileTrackingRepository, IFileRepository fileRepositoy, IStorageRepository storageRepository, IUnitOfWork unitOfWork, CancellationToken cancellationToken) {
            try {
                await unitOfWork.ExecuteOptimisticTransactionAsync(async () => {
                    var fileTrackings = await fileTrackingRepository.PollFilesToRemoveAsync(
                        _config.FetchCount, cancellationToken);

                    if (fileTrackings.Count() > 0) {
                        foreach (var fileTracking in fileTrackings) {
                            await RemoveFile(fileRepositoy, storageRepository, fileTracking);
                        }

                        await unitOfWork.CommitAsync();
                    }
                });
            } catch (Exception ex) {
                _logger.LogError(ex, "An error occurred when removing unused files");
            }
        }

        private async Task RemoveFile (IFileRepository fileRepositoy, IStorageRepository storageRepository, FileTracking fileTracking) {
            try {
                await storageRepository.DeleteFileAsync(
                    fileTracking.Category, fileTracking.ContentType, fileTracking.FileName, fileTracking.OriginalFileName);

                var storedFile = await fileRepositoy.GetFileByTrackingIdAsync(fileTracking.TrackingId);
                if (storedFile != null) {
                    await fileRepositoy.RemoveFileAsync(storedFile);
                }
            } catch (Exception ex) {
                if (fileTracking.RemovalRetryCount >= _config.MaxRetryCount) {
                    fileTracking.SetFailedToRemove();
                    _logger.LogError(ex, "File removal ({TrackingId}) failed. Exceeded max retry count.", fileTracking.TrackingId);
                } else {
                    fileTracking.RetryToRemove(TimeSpan.FromSeconds(_config.RetryDelaySeconds));
                    _logger.LogWarning(ex, "File removal ({TrackingId}) failed. Retry later.", fileTracking.TrackingId);
                }
                return;
            }

            fileTracking.SetRemoved();
            _logger.LogInformation("File removal ({TrackingId}) successful.", fileTracking.TrackingId);
        }

    }
}
