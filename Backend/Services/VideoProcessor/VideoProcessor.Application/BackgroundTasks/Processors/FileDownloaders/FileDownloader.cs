using Microsoft.Extensions.Options;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.Utilities;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.BackgroundTasks.Processors.FileDownloaders {
    public class FileDownloader : IFileDownloader {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly StorageConfiguration _config;

        public FileDownloader (IHttpClientFactory httpClientFactory, IOptions<StorageConfiguration> config) {
            _httpClientFactory = httpClientFactory;
            _config = config.Value;
        }

        public async Task<string> DownloadVideoAsync (IReadOnlyVideo video, string tempDirPath, CancellationToken cancellationToken) {
            using var httpClient = _httpClientFactory.CreateClient(HttpClients.StorageClient);
            var downloadStream = await httpClient.GetStreamAsync(GetVideoFileUrl(video), cancellationToken);

            DirectoryInfo dirInfo = new DirectoryInfo(tempDirPath);
            if (!dirInfo.Exists) {
                dirInfo.Create();
            }

            var videoPath = Path.Combine(tempDirPath, video.Id + Path.GetExtension(video.OriginalFileName));

            using (var fileStream = File.Create(videoPath)) {
                await downloadStream.CopyToAsync(fileStream, cancellationToken);
            }

            return videoPath;
        }

        private Uri GetVideoFileUrl (IReadOnlyVideo video) {
            if (string.IsNullOrEmpty(_config.BaseUri) ||
                video.VideoFileUrl.StartsWith("http://") ||
                video.VideoFileUrl.StartsWith("https://")) {
                return new Uri(video.VideoFileUrl);
            } else {
                var baseUri = new Uri(_config.BaseUri);
                return new Uri(baseUri, video.VideoFileUrl);
            }
        }

    }
}
