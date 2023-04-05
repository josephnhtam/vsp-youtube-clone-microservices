using VideoProcessor.Application.Services;

namespace VideoProcessor.Application.BackgroundTasks {
    public partial class VideoProcessingService {
        private class ConcurrentProcessesMetricsUpdater : IAsyncDisposable {
            private readonly VideoProcessingService _service;
            private readonly IKubernetesPodUpdater? _podUpdater;

            public ConcurrentProcessesMetricsUpdater (VideoProcessingService service) {
                _service = service;
                _podUpdater = service._services.GetService<IKubernetesPodUpdater>();
            }

            public async Task Execute () {
                await UpdateMetricsAsync(true);
            }

            public async ValueTask DisposeAsync () {
                await UpdateMetricsAsync(false);
            }

            private async Task UpdateMetricsAsync (bool start) {
                lock (_service) {
                    _service._concurrentProcesses += start ? 1 : -1;

                    _service._concurrentVideoProcessingTasksGauge
                        .Set(_service._concurrentProcesses);

                    _service._concurrentVideoProcessingTasksPercentageGauge
                        .Set(100f * _service._concurrentProcesses / _service._config.MaxConcurrentProcessingLimit);
                }

                if (_podUpdater != null) {
                    try {
                        _service._logger.LogInformation("Updating pod-deletion-cost annotation");

                        await _podUpdater.SetAnnotationAsync(
                            "controller.kubernetes.io/pod-deletion-cost",
                            _service._concurrentProcesses.ToString()
                        );

                        _service._logger.LogInformation("pod-deletion-cost annotation updated");
                    } catch (Exception ex) {
                        _service._logger.LogError(ex, "Failed to update pod-deletion-cost annotation");
                    }
                }
            }
        }
    }
}
