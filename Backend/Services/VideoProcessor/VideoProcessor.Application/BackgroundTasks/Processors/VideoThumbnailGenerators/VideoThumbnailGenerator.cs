using Microsoft.Extensions.Options;
using SharedKernel.Utilities;
using System.Text;
using VideoProcessor.Application.BackgroundTasks.Processors.FileUploaders;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.Utilities;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoThumbnailGenerators {
    public class VideoThumbnailGenerator : IVideoThumbnailGenerator {

        private readonly IFileUploader _fileUploader;
        private readonly VideoProcessorConfiguration _processorConfig;
        private readonly IConfiguration _config;
        private readonly ILogger<VideoThumbnailGenerator> _logger;

        public VideoThumbnailGenerator (
            IFileUploader fileUploader,
            IOptions<VideoProcessorConfiguration> processorConfig,
            IConfiguration config,
            ILogger<VideoThumbnailGenerator> logger) {
            _fileUploader = fileUploader;
            _processorConfig = processorConfig.Value;
            _config = config;
            _logger = logger;
        }

        public async Task<List<VideoThumbnail>> GenerateAsync (
            IReadOnlyVideo video,
            VideoInfo videoInfo,
            string videoFilePath,
            string tempDirPath,
            CancellationToken cancellationToken = default) {

            List<VideoThumbnail> thumbnails = new List<VideoThumbnail>();

            var thumbnailPositions = _processorConfig.ThumbnailPositions.Select(
                x => {
                    if (x.Seconds != null) {
                        return Math.Clamp(x.Seconds.Value, 0, videoInfo.LengthSeconds - 1);
                    } else {
                        return (int)Math.Ceiling(Math.Max(0, videoInfo.LengthSeconds - 1) * (x.TimePercentage ?? 0f));
                    }
                });

            foreach (var position in thumbnailPositions) {
                Guid thumbnailId = Guid.NewGuid();
                string outputThumbnailPath = Path.Combine(tempDirPath, thumbnailId.ToString() + ".png");

                int height = Math.Min(videoInfo.Height, _processorConfig.ThumbnailHeight);
                int width = (int)Math.Ceiling((float)height * ((float)videoInfo.Width / videoInfo.Height));
                width = (int)(Math.Ceiling(width / 2f) * 2f);

                var ffmpegProcess = new ChildProcess(FFmpegHelper.FFmpegExecutablesPath!, "ffmpeg");
                string args = BuildArgs(videoFilePath, outputThumbnailPath, height, width, position);

                _logger.LogInformation("Generating thumbnail for video ({VideoId}). Execute FFmpeg with args ({Args})", video.Id, args);

                if (await ffmpegProcess.RunAsync(args: args, cancellationToken: cancellationToken) != 0) {
                    _logger.LogError("Failed to generate thumbnail for video ({VideoId})", video.Id);
                    throw new Exception("Failed to generate thumbnail");
                }

                Guid groupId = video.Id;
                string uploadUrl = _config.GetValue<string>("Urls:ThumbnailUpload")!;

                var url = await _fileUploader.UploadFileToStorageAsync(
                    thumbnailId,
                    groupId,
                    Categories.ServiceUploadedVideoThumbnail,
                    outputThumbnailPath,
                    "image/png",
                    uploadUrl,
                    cancellationToken);

                thumbnails.Add(VideoThumbnail.Create(thumbnailId, position.ToString(), width, height, url));
            }

            return thumbnails;
        }

        private string BuildArgs (string videoFilePath, string outputThumbnailPath, int height, int width, int positionSeconds) {
            var argsBuilder = new StringBuilder();

            argsBuilder.Append("-y ");
            argsBuilder.Append($@"-i ""{videoFilePath}"" ");
            argsBuilder.Append($"-s {width}x{height} ");
            argsBuilder.Append($"-ss {ToFFmpegTime(TimeSpan.FromSeconds(positionSeconds))} ");
            argsBuilder.Append("-frames:v 1 ");
            argsBuilder.Append($"-threads 1 ");
            argsBuilder.Append($@"""{outputThumbnailPath}""");

            return argsBuilder.ToString();
        }

        private static string ToFFmpegTime (TimeSpan timespan) {
            return $"{(int)timespan.TotalHours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}.{timespan.Milliseconds:000}";
        }

    }
}
