using Application.Handlers;
using Microsoft.Extensions.Options;
using Storage.API.Application.Configurations;
using Storage.API.Application.Services;
using Storage.Domain.Models;

namespace Storage.API.Application.Commands.Handlers {
    public class ServiceUploadVideoCommandHandler : ICommandHandler<ServiceUploadVideoCommand, StoredFile> {

        private readonly IFileStorageHandler _fileStorageHandler;
        private readonly VideoStorageConfiguration _config;
        private readonly ILogger<ServiceUploadVideoCommandHandler> _logger;

        public ServiceUploadVideoCommandHandler (IFileStorageHandler storeFileHandler, IOptions<VideoStorageConfiguration> config, ILogger<ServiceUploadVideoCommandHandler> logger) {
            _fileStorageHandler = storeFileHandler;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<StoredFile> Handle (ServiceUploadVideoCommand request, CancellationToken cancellationToken) {
            var result = await _fileStorageHandler.StoreFileAsync(
               fileId: request.FileId,
               groupId: request.GroupId,
               category: request.Category,
               fileUploadSection: request.VideoSection,
               setFileInUseImmediately: true,
               fileRemovalDelayHours: null,
               creatorId: null,
               allowedContentTypes: null,
               maxFileSize: null,
               bufferSize: _config.BufferSize,
               onStoredFileCreated: null,
               cancellationToken: cancellationToken);

            _logger.LogInformation("Video file ({FileId}) ({GroudId}) is uploaded from service", request.FileId, request.GroupId);

            return result;
        }

    }
}
