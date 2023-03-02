using Application.Handlers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Exceptions;
using SharedKernel.Utilities;
using Storage.API.Application.Configurations;
using Storage.API.Application.DtoModels;
using Storage.API.Application.Services;
using Storage.Domain.Models;
using System.Security.Claims;

namespace Storage.API.Application.Commands.Handlers {
    public class UserUploadImageCommandHandler : ICommandHandler<UserUploadImageCommand, ImageUploadResponseDto> {

        private readonly IFileStorageHandler _fileStorageHandler;
        private readonly IImageFormatChecker _imageFormatChecker;
        private readonly ImageUploadTokenValidationConfiguration _tokenValidationConfig;
        private readonly ImageStorageConfiguration _storageConfig;
        private readonly ILogger<UserUploadImageCommandHandler> _logger;

        public readonly static List<string> AllowedMimeTypes = new List<string> {
            "image/png",
            "image/jpeg"
        };

        public UserUploadImageCommandHandler (
            IFileStorageHandler storeFileHandler,
            IImageFormatChecker imageFormatChecker,
            IOptions<ImageUploadTokenValidationConfiguration> tokenValidationConfig,
            IOptions<ImageStorageConfiguration> storageConfig,
            ILogger<UserUploadImageCommandHandler> logger) {
            _fileStorageHandler = storeFileHandler;
            _imageFormatChecker = imageFormatChecker;
            _tokenValidationConfig = tokenValidationConfig.Value;
            _storageConfig = storageConfig.Value;
            _logger = logger;
        }

        public async Task<ImageUploadResponseDto> Handle (UserUploadImageCommand request, CancellationToken cancellationToken) {
            var token = request.Token;
            var imageSection = request.ImageSection;

            var (imageId, category, creatorId, maxFileSize) = Validate(request, token);

            if (request.UserId != creatorId) {
                throw new AppException("Unauthorized", null, StatusCodes.Status401Unauthorized);
            }

            var storedFile = await _fileStorageHandler.StoreFileAsync(
                fileId: imageId,
                groupId: imageId,
                category: category,
                fileUploadSection: imageSection,
                setFileInUseImmediately: false,
                fileRemovalDelayHours: null,
                creatorId: creatorId,
                allowedContentTypes: AllowedMimeTypes,
                maxFileSize: maxFileSize,
                bufferSize: _storageConfig.BufferSize,
                onStoredFileCreated: null,
                fileValidator: ValidateImageFile,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Image file ({FileId}) ({GroudId}) is uploaded from user ({UserId})", imageId, imageId, creatorId);

            return new ImageUploadResponseDto {
                FileId = storedFile.FileId,
                Category = storedFile.Category,
                ContentType = storedFile.ContentType,
                Url = storedFile.Url,
                FileSize = storedFile.FileSize,
                Token = CreateFileToken(storedFile)
            };
        }

        private string CreateFileToken (StoredFile storedFile) {
            return JwtHelper.GenerateJWT(
                _storageConfig.SecretKey,
                SecurityAlgorithms.HmacSha256Signature,
                (descriptor) => {
                    descriptor.Issuer = _storageConfig.Issuer;
                    descriptor.Expires = DateTime.UtcNow + TimeSpan.FromMinutes(_storageConfig.ExpireMinutes);
                    descriptor.Subject = new ClaimsIdentity(new List<Claim> {
                        new Claim("file_id", storedFile.FileId.ToString()),
                        new Claim("file_name", storedFile.FileName),
                        new Claim("original_file_name", storedFile.OriginalFileName),
                        new Claim("file_size", storedFile.FileSize.ToString()),
                        new Claim("url", storedFile.Url),
                        new Claim("category", storedFile.Category),
                        new Claim("content_type", storedFile.ContentType??""),
                        new Claim("user_id", storedFile.UserId??"")
                    });
                }
            );
        }

        private async Task ValidateImageFile (FileStream fileStream) {
            string? contentType = await _imageFormatChecker.GetContentTypeAsync(fileStream);

            if (string.IsNullOrEmpty(contentType) || !AllowedMimeTypes.Contains(contentType)) {
                throw new Exception("Failed to validate image format");
            }
        }

        private (Guid imageId, string category, string creatorId, long? maxSize) Validate (UserUploadImageCommand request, string token) {
            if (!JwtHelper.ValidateJWT(token, _tokenValidationConfig.SecretKey, out var claimsPrincipal, (parameters) => {
                parameters.ValidateIssuer = true;
                parameters.ValidateAudience = false;
                parameters.ValidateLifetime = true;

                parameters.ValidIssuers = _tokenValidationConfig.Issuers;
            })) {
                throw new AppException("Failed to validate token", null, StatusCodes.Status401Unauthorized);
            }

            var userId = claimsPrincipal.FindFirstValue("user_id");
            var imageIdString = claimsPrincipal.FindFirstValue("image_id");
            var category = claimsPrincipal.FindFirstValue("category");
            var maxSizeString = claimsPrincipal.FindFirstValue("max_size");

            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(category) ||
                string.IsNullOrEmpty(imageIdString) ||
                request.UserId != userId) {
                throw new AppException("The token is invalid", null, StatusCodes.Status401Unauthorized);
            }

            var imageId = Guid.Parse(imageIdString);

            long? maxSize = null;
            if (long.TryParse(maxSizeString, out long value)) {
                maxSize = value;
            }

            return (imageId, category, userId, maxSize);
        }
    }
}
