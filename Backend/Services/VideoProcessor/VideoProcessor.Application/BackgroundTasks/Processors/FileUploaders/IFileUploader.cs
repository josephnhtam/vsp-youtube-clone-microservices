namespace VideoProcessor.Application.BackgroundTasks.Processors.FileUploaders {
    public interface IFileUploader {
        Task<string> UploadFileToStorageAsync (
            Guid fileId,
            Guid groupId,
            string category,
            string filePath,
            string contentType,
            string uploadUrl,
            CancellationToken cancellationToken);
    }
}
