using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoPreviewThumbnailGenerators {
    public interface IVideoPreviewThumbnailGenerator {
        Task<VideoPreviewThumbnail> GenerateAsync (
            IReadOnlyVideo video,
            VideoInfo videoInfo,
            string videoFilePath,
            string tempDirPath,
            CancellationToken cancellationToken = default);
    }
}
