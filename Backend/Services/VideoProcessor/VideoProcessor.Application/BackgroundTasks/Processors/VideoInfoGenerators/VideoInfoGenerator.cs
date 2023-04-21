using Serilog.Context;
using SharedKernel.Utilities;
using System.Text;
using System.Text.Json;
using VideoProcessor.Application.Utilities;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoInfoGenerators {
    public class VideoInfoGenerator : IVideoInfoGenerator {

        private readonly ILogger<VideoInfoGenerator> _logger;

        public VideoInfoGenerator (ILogger<VideoInfoGenerator> logger) {
            _logger = logger;
        }

        public async Task<VideoInfo> GenerateAsync (IReadOnlyVideo video, string videoFilePath, CancellationToken cancellationToken) {
            var ffprobeProcess = new ChildProcess(FFmpegHelper.FFmpegExecutablesPath!, "ffprobe");

            var args = $@"-v panic -print_format json -show_format -show_streams ""{videoFilePath}""";

            JsonDocument? outputDoc = null;

            if (await ffprobeProcess.RunAsync(
                args: args,
                processOutput: ProcessOutput,
                cancellationToken: cancellationToken) != 0) {
                _logger.LogError("Failed to generate info for video ({VideoId})", video.Id);
                throw new Exception("Failed to generate video info");
            }

            if (outputDoc == null) {
                _logger.LogError("Failed to get the metadata from video ({VideoId})", video.Id);
                throw new Exception("Failed to get the metadata from video");
            }

            (int width, int height) resolution = GetResolution(outputDoc);
            long size = GetSize(outputDoc);
            int duration = (int)Math.Ceiling(GetDuration(outputDoc));

            return VideoInfo.Create(resolution.width, resolution.height, size, duration);

            async Task ProcessOutput (Stream stream, IProcessHandle processHandle, CancellationToken cancellationToken) {
                StringBuilder outputJsonBuilder = new StringBuilder();

                using (var reader = new StreamReader(stream)) {
                    string? line;
                    while ((line = await reader.ReadLineAsync(cancellationToken)) != null) {
                        outputJsonBuilder.AppendLine(line);
                    }

                    await processHandle.WaitForExitAsync();

                    if (processHandle.IsEndedSuccessfully()) {
                        string outputJson = outputJsonBuilder.ToString();
                        outputDoc = JsonSerializer.Deserialize<JsonDocument>(outputJson);
                        _logger.LogInformation("FFprobe process is ended successfully for video ({VideoId})", video.Id);
                    } else {
                        using var isCancelledProperty = LogContext.PushProperty("IsCancelled", processHandle.IsCancelled);
                        _logger.LogError("FFprobe process is ended unsuccessfully with exit code ({ExitCode}) for video ({VideoId})", processHandle.ExitCode, video.Id);
                    }
                }
            }
        }

        private (int width, int height) GetResolution (JsonDocument outputDoc) {
            var streamsArray = outputDoc.RootElement.GetProperty("streams");

            (int width, int height) resolution = streamsArray.EnumerateArray()
                .Where(stream => stream.GetProperty("codec_type").GetString() == "video")
                .Select(stream => (stream.GetProperty("width").GetInt32(), stream.GetProperty("height").GetInt32()))
                .OrderByDescending(resolution => resolution.Item1 * resolution.Item2)
                .FirstOrDefault();

            return (resolution.width, resolution.height);
        }

        private long GetSize (JsonDocument outputDoc) {
            return long.Parse(outputDoc.RootElement.GetProperty("format").GetProperty("size").GetString()!);
        }

        private double GetDuration (JsonDocument outputDoc) {
            return double.Parse(outputDoc.RootElement.GetProperty("format").GetProperty("duration").GetString()!);
        }

    }
}
