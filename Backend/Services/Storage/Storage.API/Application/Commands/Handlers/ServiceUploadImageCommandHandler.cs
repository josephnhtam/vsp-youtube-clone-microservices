using Application.Handlers;
using Microsoft.Extensions.Options;
using Storage.API.Application.Configurations;
using Storage.API.Application.Services;
using Storage.Domain.Models;

namespace Storage.API.Application.Commands.Handlers {
    public class ServiceUploadImageCommandHandler : ICommandHandler<ServiceUploadImageCommand, StoredFile> {

        private readonly IFileStorageHandler _fileStorageHandler;
        private readonly ImageStorageConfiguration _config;
        private readonly ILogger<ServiceUploadImageCommandHandler> _logger;

        public ServiceUploadImageCommandHandler (IFileStorageHandler storeFileHandler, IOptions<ImageStorageConfiguration> config, ILogger<ServiceUploadImageCommandHandler> logger) {
            _fileStorageHandler = storeFileHandler;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<StoredFile> Handle (ServiceUploadImageCommand request, CancellationToken cancellationToken) {
            var result = await _fileStorageHandler.StoreFileAsync(
               fileId: request.FileId,
               groupId: request.GroupId,
               category: request.Category,
               fileUploadSection: request.ImageSection,
               setFileInUseImmediately: true,
               fileRemovalDelayHours: null,
               creatorId: null,
               allowedContentTypes: null,
               maxFileSize: null,
               bufferSize: _config.BufferSize,
               onStoredFileCreated: null,
               cancellationToken: cancellationToken);

            _logger.LogInformation("Image file ({FileId}) ({GroudId}) is uploaded from service", request.FileId, request.GroupId);

            return result;
        }

    }
}
