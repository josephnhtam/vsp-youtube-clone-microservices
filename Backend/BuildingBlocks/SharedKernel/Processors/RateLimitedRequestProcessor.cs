using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SharedKernel.Processors {
    public class RateLimitedRequestProcessor : IDisposable {

        private readonly SemaphoreSlim _semaphore;
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<ulong, Task> _tasks;
        private readonly ILogger? _logger;

        private int _processedRequestCount;
        private ulong _totalRequestCount;

        public RateLimitedRequestProcessor (RateLimitedRequestProcessorOptions options, ILogger? logger = null) {
            if (options.MaxConcurrentProcessingLimit < 1)
                throw new ArgumentOutOfRangeException(nameof(options.MaxConcurrentProcessingLimit));

            if (options.MaxProcessingRateLimit <= 0f)
                throw new ArgumentOutOfRangeException(nameof(options.MaxProcessingRateLimit));

            var maxConcurrentProcessingLimit = options.MaxConcurrentProcessingLimit;
            var maxProcessingRateLimit = options.MaxProcessingRateLimit;
            _logger = logger;

            _semaphore = new SemaphoreSlim(maxConcurrentProcessingLimit);
            TimeSpan releaseInterval = TimeSpan.FromSeconds(1f / maxProcessingRateLimit);
            _timer = new Timer(ReleaseSemaphore, null, releaseInterval, releaseInterval);

            _tasks = new ConcurrentDictionary<ulong, Task>();
        }

        public async Task RunAsync (Func<Task> request, CancellationToken cancellationToken = default) {
            await Task.Yield();

            try {
                while (!cancellationToken.IsCancellationRequested) {
                    await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                    var taskId = Interlocked.Increment(ref _totalRequestCount);
                    _tasks[taskId] = Task.Run(async () => await ProcessRequestAsync(request, cancellationToken, taskId));
                }
            } catch (OperationCanceledException) {
                _logger?.LogInformation("Rate-Limited Request Processor is stopping");
            }

            await Task.WhenAll(_tasks.Values).ConfigureAwait(false);

            _logger?.LogInformation("Rate-Limited Request Processor is stopped");
        }

        private async Task ProcessRequestAsync (Func<Task> request, CancellationToken cancellationToken, ulong taskId) {
            try {
                await request.Invoke();
            } catch (Exception ex) {
                if (_logger != null) {
                    _logger.LogError(ex, "An exception is thrown from request processing");
                }
            } finally {
                Interlocked.Increment(ref _processedRequestCount);
                _tasks.TryRemove(taskId, out var _);
            }
        }

        private void ReleaseSemaphore (object? state) {
            if (_processedRequestCount > 0) {
                Interlocked.Decrement(ref _processedRequestCount);
                _semaphore.Release();
            }
        }

        public void Dispose () {
            _semaphore.Dispose();
            _timer.Dispose();
        }
    }
}
