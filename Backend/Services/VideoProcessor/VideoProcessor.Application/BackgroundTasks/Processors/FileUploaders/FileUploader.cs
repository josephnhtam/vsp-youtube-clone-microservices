using System.Net.Http.Headers;
using VideoProcessor.Application.Utilities;

namespace VideoProcessor.Application.BackgroundTasks.Processors.FileUploaders {
    public class FileUploader : IFileUploader {

        private readonly IHttpClientFactory _httpClientFactory;

        public FileUploader (IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> UploadFileToStorageAsync (
            Guid fileId,
            Guid groupId,
            string category,
            string filePath,
            string contentType,
            string uploadUrl,
            CancellationToken cancellationToken) {

            using var formData = new MultipartFormDataContent();

            var videoContent = new StreamContent(File.OpenRead(filePath));
            videoContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            formData.Add(videoContent, "file", Path.GetFileName(filePath));

            using var httpClient = _httpClientFactory.CreateClient(HttpClients.StorageClient);

            httpClient.DefaultRequestHeaders.Add("FileId", fileId.ToString());
            httpClient.DefaultRequestHeaders.Add("GroupId", groupId.ToString());
            httpClient.DefaultRequestHeaders.Add("Category", category);

            var response = await httpClient.PostAsync(uploadUrl, formData, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

    }
}
