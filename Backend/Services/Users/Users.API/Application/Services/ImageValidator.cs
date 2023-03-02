using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;
using SharedKernel.Utilities;
using System.Security.Claims;
using Users.API.Application.Configurations;
using Users.Domain.Models;

namespace Users.API.Application.Services {
    public class ImageValidator : IImageValidator {

        private readonly ImageTokenValidationConfiguration _tokenValidationConfig;

        public ImageValidator (IOptions<ImageTokenValidationConfiguration> tokenValidationConfig) {
            _tokenValidationConfig = tokenValidationConfig.Value;
        }

        public ImageFile ValidateImageToken (string token, string? validCategory, string validUserId) {
            if (!JwtHelper.ValidateJWT(token, _tokenValidationConfig.SecretKey, out var claimsPrincipal, (parameters) => {
                parameters.ValidateIssuer = true;
                parameters.ValidateAudience = false;
                parameters.ValidateLifetime = true;

                parameters.ValidIssuers = _tokenValidationConfig.Issuers;
            })) {
                throw new AppException("Failed to validate token", null, StatusCodes.Status401Unauthorized);
            }

            var userId = claimsPrincipal.FindFirstValue("user_id");
            var category = claimsPrincipal.FindFirstValue("category");
            var imageFileIdString = claimsPrincipal.FindFirstValue("file_id");
            var url = claimsPrincipal.FindFirstValue("url");

            if (userId == null || imageFileIdString == null || validUserId != userId || (validCategory != null && validCategory != category)) {
                throw new AppException("The token is invalid", null, StatusCodes.Status400BadRequest);
            }

            var imageFileId = Guid.Parse(imageFileIdString);

            return ImageFile.Create(imageFileId, url);
        }
    }
}
