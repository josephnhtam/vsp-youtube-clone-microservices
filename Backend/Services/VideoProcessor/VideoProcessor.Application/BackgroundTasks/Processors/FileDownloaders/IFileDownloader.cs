using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.FileDownloaders {
    public interface IFileDownloader {
        Task<string> DownloadVideoAsync (Video video, string tempDirPath, CancellationToken cancellationToken);
    }
}
