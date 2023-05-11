using Application.Handlers;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;
using SharedKernel.Utilities;
using Storage.API.Application.Configurations;
using Storage.API.Application.IntegrationEvents;
using Storage.API.Application.Services;
using Storage.Domain.Models;
using System.Security.Claims;

namespace Storage.API.Application.Commands.Handlers {
    public class UserUploadVideoCommandHandler : ICommandHandler<UserUploadVideoCommand, StoredFile> {

        private readonly IFileStorageHandler _fileStorageHandler;
        private readonly VideoUploadTokenValidationConfiguration _tokenValidationConfig;
        private readonly VideoStorageConfiguration _storageConfig;
        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly ILogger<UserUploadVideoCommandHandler> _logger;

        public readonly static List<string> AllowedMimeTypes = new List<string> {
            "video/mp4",
            "video/quicktime",
            "video/x-quicktime",
            "video/mpeg",
            "video/webm",
            "video/3gpp",
            "video/3gpp2",
            "video/x-flv",
            "video/x-matroska",
            "video/x-ms-wmv",
            "video/x-msvideo",
            "video/ogg",
            "application/vnd.rn-realmedia-vbr"
        };

        public UserUploadVideoCommandHandler (
            IFileStorageHandler storeFileHandler,
            ITransactionalEventsContext transactionalEventsContext,
            IOptions<VideoUploadTokenValidationConfiguration> tokenValidationConfig,
            IOptions<VideoStorageConfiguration> storageConfig,
            ILogger<UserUploadVideoCommandHandler> logger) {
            _fileStorageHandler = storeFileHandler;
            _transactionalEventsContext = transactionalEventsContext;
            _tokenValidationConfig = tokenValidationConfig.Value;
            _storageConfig = storageConfig.Value;
            _logger = logger;
        }

        public async Task<StoredFile> Handle (UserUploadVideoCommand request, CancellationToken cancellationToken) {
            var token = request.Token;
            var videoSection = request.VideoSection;

            var (videoId, category, creatorId, maxSize) = Validate(request, token);

            if (request.UserId != creatorId) {
                throw new AppException("Unauthorized", null, StatusCodes.Status401Unauthorized);
            }

            var result = await _fileStorageHandler.StoreFileAsync(
               fileId: videoId,
               groupId: videoId,
               category: category,
               fileUploadSection: videoSection,
               setFileInUseImmediately: true,
               fileRemovalDelayHours: null,
               creatorId: creatorId,
               allowedContentTypes: AllowedMimeTypes,
               maxFileSize: maxSize,
               bufferSize: _storageConfig.BufferSize,
               onStoredFileCreated: OnStoredFileCreated,
               cancellationToken: cancellationToken);

            _logger.LogInformation("Video file ({FileId}) ({GroudId}) is uploaded from user ({UserId})", videoId, videoId, creatorId);

            return result;
        }

        private Task OnStoredFileCreated (StoredFile storedFile) {
            _transactionalEventsContext.AddOutboxMessage(
                new VideoUploadedIntegrationEvent(
                    storedFile.FileId,
                    storedFile.UserId ?? string.Empty,
                    storedFile.OriginalFileName,
                    storedFile.Url));

            return Task.CompletedTask;
        }

        private (Guid videoId, string category, string creatorId, long? maxSize) Validate (UserUploadVideoCommand request, string token) {
            if (!JwtHelper.ValidateJWT(token, _tokenValidationConfig.SecretKey, out var claimsPrincipal, (parameters) => {
                parameters.ValidateIssuer = true;
                parameters.ValidateAudience = false;
                parameters.ValidateLifetime = true;

                parameters.ValidIssuers = _tokenValidationConfig.Issuers;
            })) {
                throw new AppException("Failed to validate token", null, StatusCodes.Status401Unauthorized);
            }

            var videoIdString = claimsPrincipal.FindFirstValue("video_id");
            var category = claimsPrincipal.FindFirstValue("category");
            var creatorId = claimsPrincipal.FindFirstValue("creator_id");
            var maxSizeString = claimsPrincipal.FindFirstValue("max_size");

            if (string.IsNullOrEmpty(creatorId) ||
                string.IsNullOrEmpty(category) ||
                string.IsNullOrEmpty(videoIdString) ||
                request.UserId != creatorId) {
                throw new AppException("The token is invalid", null, StatusCodes.Status401Unauthorized);
            }

            var videoId = Guid.Parse(videoIdString);

            long? maxSize = null;
            if (long.TryParse(maxSizeString, out long value)) {
                maxSize = value;
            }

            return (videoId, category, creatorId, maxSize);
        }
    }
}
