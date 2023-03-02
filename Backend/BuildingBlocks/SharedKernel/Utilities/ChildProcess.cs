using System.Diagnostics;

namespace SharedKernel.Utilities {
    public class ChildProcess {

        private readonly string executablePath;

        public ChildProcess (string dirPath, string executableName) {
            if (string.IsNullOrEmpty(dirPath)) {
                throw new ArgumentException("Directory path cannot be null or empty.", nameof(dirPath));
            }

            if (string.IsNullOrEmpty(executableName)) {
                throw new ArgumentException("Executable name cannot be null or empty.", nameof(executableName));
            }

            var dir = new DirectoryInfo(dirPath);
            var executable = dir.GetFiles().FirstOrDefault((info) =>
                info.Name.Equals(executableName, StringComparison.InvariantCultureIgnoreCase) ||
                info.Name.Equals(executableName + ".exe", StringComparison.InvariantCultureIgnoreCase));

            if (executable == null) throw new Exception($"Executable ({executableName}) not found.");

            executablePath = executable.FullName;
        }

        public async Task<int> RunAsync (
            string? args = null,
            Action<StreamWriter, IProcessHandle, CancellationToken>? input = null,
            Func<Stream, IProcessHandle, CancellationToken, Task>? processError = null,
            Func<Stream, IProcessHandle, CancellationToken, Task>? processOutput = null,
            ProcessPriorityClass? priority = null,
            TimeSpan? shutdownTimeout = null,
            CancellationToken cancellationToken = default) {
            using (var process = new Process()) {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)) {
                    var tasks = new List<Task>();

                    try {
                        process.StartInfo = new ProcessStartInfo {
                            FileName = executablePath,
                            Arguments = args ?? string.Empty,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardInput = input != null,
                            RedirectStandardError = processError != null,
                            RedirectStandardOutput = processOutput != null,
                        };

                        var processStatus = new ProcessHandle(process, cancellationToken);

                        if (!process.Start()) {
                            throw new InvalidOperationException("Process cannot be started.");
                        }

                        if (priority.HasValue) {
                            try {
                                process.PriorityClass = priority.Value;
                            } catch (Exception) { }
                        }

                        if (input != null) {
                            input.Invoke(process.StandardInput, processStatus, cts.Token);
                        }

                        if (processError != null) {
                            tasks.Add(processError.Invoke(process.StandardError.BaseStream, processStatus, cancellationToken));
                        }

                        if (processOutput != null) {
                            tasks.Add(processOutput.Invoke(process.StandardOutput.BaseStream, processStatus, cancellationToken));
                        }

                        await process.WaitForExitAsync(cancellationToken);
                    } catch (Exception) {
                        if (!cts.IsCancellationRequested) {
                            cts.Cancel();
                        }

                        if (!process.HasExited) {
                            bool signalSent = process.CloseMainWindow();

                            if (signalSent) {
                                using var shutdownCts = new CancellationTokenSource(
                                    shutdownTimeout ?? TimeSpan.FromSeconds(5)
                                );
                                try {
                                    await process.WaitForExitAsync(shutdownCts.Token);
                                } catch (OperationCanceledException) { }
                            }

                            if (!process.HasExited) {
                                process.Kill();
                            }
                        }

                        throw;
                    } finally {
                        if (!cts.IsCancellationRequested) {
                            cts.Cancel();
                        }

                        await Task.WhenAll(tasks);
                    }
                }

                return process.ExitCode;
            }
        }

    }

    public interface IProcessHandle {
        int Id { get; }
        int ExitCode { get; }
        bool HasExited { get; }
        DateTime StartTime { get; }
        DateTime ExitTime { get; }
        public ProcessPriorityClass PriorityClass { get; }
        public TimeSpan PrivilegedProcessorTime { get; }
        public TimeSpan TotalProcessorTime { get; }
        public TimeSpan UserProcessorTime { get; }
        public bool IsCancelled { get; }
        public Task WaitForExitAsync (CancellationToken cancellationToken = default);

        bool IsEndedSuccessfully ();
        bool IsEndedSuccessfully (params int[] successfulExitCode);
    }

    public class ProcessHandle : IProcessHandle {
        private readonly Process _process;
        private readonly CancellationToken _cancellationToken;

        public ProcessHandle (Process process, CancellationToken cancellationToken) {
            _process = process;
            _cancellationToken = cancellationToken;
        }

        public int Id => _process.Id;
        public int ExitCode => _process.ExitCode;
        public bool HasExited => _process.HasExited;
        public DateTime StartTime => _process.StartTime;
        public DateTime ExitTime => _process.ExitTime;
        public ProcessPriorityClass PriorityClass => _process.PriorityClass;
        public TimeSpan PrivilegedProcessorTime => _process.PrivilegedProcessorTime;
        public TimeSpan TotalProcessorTime => _process.TotalProcessorTime;
        public TimeSpan UserProcessorTime => _process.UserProcessorTime;
        public bool IsCancelled => _cancellationToken.IsCancellationRequested;

        public bool IsEndedSuccessfully () {
            return IsEndedSuccessfully(0);
        }

        public bool IsEndedSuccessfully (params int[] successfulExitCode) {
            return HasExited && successfulExitCode.Contains(ExitCode) && !IsCancelled;
        }

        public async Task WaitForExitAsync (CancellationToken cancellationToken) {
            await _process.WaitForExitAsync(cancellationToken);
        }
    }
}
