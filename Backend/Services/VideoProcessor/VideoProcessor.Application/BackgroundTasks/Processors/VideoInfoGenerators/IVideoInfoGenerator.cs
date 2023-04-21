using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoInfoGenerators {
    public interface IVideoInfoGenerator {
        Task<VideoInfo> GenerateAsync (IReadOnlyVideo video, string videoFilePath, CancellationToken cancellationToken);
    }
}
