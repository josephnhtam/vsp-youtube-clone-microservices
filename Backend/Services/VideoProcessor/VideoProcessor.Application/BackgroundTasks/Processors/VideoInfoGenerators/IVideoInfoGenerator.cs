using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.VideoInfoGenerators {
    public interface IVideoInfoGenerator {
        Task<VideoInfo> GenerateAsync (Video video, string videoFilePath, CancellationToken cancellationToken);
    }
}
