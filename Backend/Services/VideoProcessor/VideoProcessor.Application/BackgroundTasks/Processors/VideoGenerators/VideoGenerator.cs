using Microsoft.Extensions.Options;
using SharedKernel.Utilities;
using System.Text;
using VideoProcessor.Application.BackgroundTasks.Processors.FileUploaders;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.Utilities;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoGenerators {
    public class VideoGenerator : IVideoGenerator {
        private readonly IFileUploader _fileUploader;
        private readonly VideoGeneratorConfiguration _generatorConfig;
        private readonly IConfiguration _config;
        private readonly ILogger<VideoGenerator> _logger;

        public VideoGenerator (
            IFileUploader fileUploader,
            IOptions<VideoGeneratorConfiguration> generatorConfig,
            IConfiguration configuration,
            ILogger<VideoGenerator> logger) {
            _fileUploader = fileUploader;
            _generatorConfig = generatorConfig.Value;
            _config = configuration;
            _logger = logger;
        }

        public async Task<ProcessedVideo?> GenerateAsync (
            IReadOnlyVideo video,
            VideoInfo videoInfo,
            string videoFilePath,
            VideoProcessingStep processingStep,
            bool required,
            string tempDirPath,
            CancellationToken cancellationToken = default) {
            var label = processingStep.Label;
            var height = processingStep.Height;

            if (videoInfo.Height < height) {
                if (required) {
                    height = videoInfo.Height;
                } else {
                    return null;
                }
            }

            int width = (int)Math.Ceiling(height * ((float)videoInfo.Width / videoInfo.Height));
            width = (int)(Math.Ceiling(width / 2f) * 2f);

            Guid processedVideoId = Guid.NewGuid();
            string outputVideoPath = Path.Combine(tempDirPath, processedVideoId.ToString() + ".mp4");

            var ffmpegProcess = new ChildProcess(FFmpegHelper.FFmpegExecutablesPath!, "ffmpeg");
            string args = BuildArgs(videoFilePath, outputVideoPath, height, width);

            _logger.LogInformation("Generating video ({Resolution}) for video ({VideoId}). Execute FFmpeg with args ({Args})", height, video.Id, args);

            if (await ffmpegProcess.RunAsync(args: args, cancellationToken: cancellationToken) != 0) {
                _logger.LogError("Failed to generate video ({Resolution}) for video ({VideoId})", height, video.Id);
                throw new Exception("Failed to generate video");
            }

            var outputVideoFile = new FileInfo(outputVideoPath);
            if (!outputVideoFile.Exists) {
                throw new Exception($"Failed to generate video at {outputVideoPath}");
            }

            Guid groupId = video.Id;
            string uploadUrl = _config.GetValue<string>("Urls:VideoUpload")!;

            var url = await _fileUploader.UploadFileToStorageAsync(
                processedVideoId,
                groupId,
                Categories.ServiceUploadedVideo,
                outputVideoPath,
                "video/mp4",
                uploadUrl,
                cancellationToken);

            return ProcessedVideo.Create(
                processedVideoId,
                label,
                width,
                height,
                outputVideoFile.Length,
                videoInfo.LengthSeconds,
                url);
        }

        private string BuildArgs (string videoFilePath, string outputVideoPath, int height, int width) {
            var argsBuilder = new StringBuilder();

            argsBuilder.Append("-y ");

            string videoEncoder;
            int threads;
            if (_generatorConfig.HardwareAcceleration.Enable) {
                videoEncoder = _generatorConfig.HardwareAcceleration.Encoder;
                threads = 1;
                argsBuilder.Append($@"-hwaccel {_generatorConfig.HardwareAcceleration.HardwareAccelerator} ");
                argsBuilder.Append($@"-c:v {_generatorConfig.HardwareAcceleration.Decoder} ");
            } else {
                threads = _generatorConfig.FFmpegThreadsCount;
                videoEncoder = "h264";
            }

            argsBuilder.Append($@"-i ""{videoFilePath}"" ");
            argsBuilder.Append($"-s {width}x{height} ");
            argsBuilder.Append($"-c:v {videoEncoder} ");
            argsBuilder.Append("-c:a aac ");
            argsBuilder.Append($"-threads {threads} ");
            argsBuilder.Append($@"""{outputVideoPath}""");

            return argsBuilder.ToString();
        }
    }
}
