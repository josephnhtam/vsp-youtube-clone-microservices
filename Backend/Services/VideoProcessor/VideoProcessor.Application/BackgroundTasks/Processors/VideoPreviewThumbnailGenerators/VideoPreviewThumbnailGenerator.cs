using Microsoft.Extensions.Options;
using SharedKernel.Utilities;
using System.Text;
using VideoProcessor.Application.BackgroundTasks.Processors.FileUploaders;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.Utilities;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoPreviewThumbnailGenerators {
    public class VideoPreviewThumbnailGenerator : IVideoPreviewThumbnailGenerator {

        private readonly IFileUploader _fileUploader;
        private readonly VideoProcessorConfiguration _processorConfig;
        private readonly VideoGeneratorConfiguration _generatorConfig;
        private readonly IConfiguration _config;
        private readonly ILogger<VideoPreviewThumbnailGenerator> _logger;

        public VideoPreviewThumbnailGenerator (
            IFileUploader fileUploader,
            IOptions<VideoProcessorConfiguration> processorConfig,
            IOptions<VideoGeneratorConfiguration> generatorConfig,
            IConfiguration config,
            ILogger<VideoPreviewThumbnailGenerator> logger) {
            _fileUploader = fileUploader;
            _processorConfig = processorConfig.Value;
            _generatorConfig = generatorConfig.Value;
            _config = config;
            _logger = logger;
        }

        public async Task<VideoPreviewThumbnail> GenerateAsync (
            IReadOnlyVideo video,
            VideoInfo videoInfo,
            string videoFilePath,
            string tempDirPath,
            CancellationToken cancellationToken = default) {

            Guid thumbnailId = Guid.NewGuid();
            string outputThumbnailPath = Path.Combine(tempDirPath, thumbnailId.ToString() + ".webp");

            int height = Math.Min(videoInfo.Height, _processorConfig.PreviewThumbnailHeight);
            int width = (int)Math.Ceiling((float)height * ((float)videoInfo.Width / videoInfo.Height));
            width = (int)(Math.Ceiling(width / 2f) * 2f);

            var totalLengthSeconds = (float)videoInfo.LengthSeconds;
            float startPositionSeconds = totalLengthSeconds * _processorConfig.PreviewThumbnailStartPosition;
            float lengthSeconds = Math.Min(totalLengthSeconds, _processorConfig.PreviewThumbnailLengthSeconds);
            if (totalLengthSeconds - startPositionSeconds < _processorConfig.PreviewThumbnailLengthSeconds) {
                startPositionSeconds = 0f;
            }

            var ffmpegProcess = new ChildProcess(FFmpegHelper.FFmpegExecutablesPath!, "ffmpeg");
            string args = BuildArgs(videoFilePath, outputThumbnailPath, height, width, startPositionSeconds, lengthSeconds);

            _logger.LogInformation("Generating preview thumbnail for video ({VideoId}). Execute FFmpeg with args ({Args})", video.Id, args);

            if (await ffmpegProcess.RunAsync(args: args, cancellationToken: cancellationToken) != 0) {
                _logger.LogError("Failed to generate preview thumbnail for video ({VideoId})", video.Id);
                throw new Exception("Failed to generate preview thumbnail");
            }

            Guid groupId = video.Id;
            string uploadUrl = _config.GetValue<string>("Urls:ThumbnailUpload")!;

            var url = await _fileUploader.UploadFileToStorageAsync(
                thumbnailId,
                groupId,
                Categories.ServiceUploadedVideoThumbnail,
                outputThumbnailPath,
                "image/webp",
                uploadUrl,
                cancellationToken);

            return VideoPreviewThumbnail.Create(thumbnailId, width, height, lengthSeconds, url);
        }

        private string BuildArgs (string videoFilePath, string outputThumbnailPath, int height, int width, float startPositionSeconds, float lengthSeconds) {
            var argsBuilder = new StringBuilder();

            argsBuilder.Append("-y ");
            argsBuilder.Append($@"-i ""{videoFilePath}"" ");
            argsBuilder.Append($"-s {width}x{height} ");
            argsBuilder.Append("-r 12.000 ");
            argsBuilder.Append("-loop 0 ");
            argsBuilder.Append("-c:v webp ");
            argsBuilder.Append($"-ss {ToFFmpegTime(TimeSpan.FromSeconds(startPositionSeconds))} ");
            argsBuilder.Append($"-t {ToFFmpegTime(TimeSpan.FromSeconds(lengthSeconds))} ");
            argsBuilder.Append($"-threads {_generatorConfig.FFmpegThreadsCount} ");
            argsBuilder.Append($@"""{outputThumbnailPath}""");

            return argsBuilder.ToString();
        }

        private static string ToFFmpegTime (TimeSpan timespan) {
            return $"{(int)timespan.TotalHours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}.{timespan.Milliseconds:000}";
        }

    }
}
