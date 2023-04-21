using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoGenerators {
    public interface IVideoGenerator {
        Task<ProcessedVideo?> GenerateAsync (
            IReadOnlyVideo video,
            VideoInfo videoInfo,
            string videoFilePath,
            VideoProcessingStep processingStep,
            bool required,
            string tempDirPath,
            CancellationToken cancellationToken = default);
    }
}
