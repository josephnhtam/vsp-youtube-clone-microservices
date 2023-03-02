
using Application.Handlers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Exceptions;
using SharedKernel.Utilities;
using System.Security.Claims;
using Users.API.Application.Configurations;
using Users.API.Application.DtoModels;
using Users.API.Application.Utilities;
using Users.Domain.Contracts;

namespace Users.API.Application.Queries.Handlers {
    public class GetBannerUploadTokenQueryHandler : IQueryHandler<GetBannerUploadTokenQuery, GetBannerUploadTokenResponseDto> {

        private readonly ImageUploadConfiguration _config;
        private readonly IUserProfileRepository _repository;

        public GetBannerUploadTokenQueryHandler (IOptions<ImageUploadConfiguration> config, IUserProfileRepository repository) {
            _config = config.Value;
            _repository = repository;
        }

        public async Task<GetBannerUploadTokenResponseDto> Handle (GetBannerUploadTokenQuery request, CancellationToken cancellationToken) {
            var userProfile = await _repository.GetUserProfileByIdAsync(request.UserId, false, cancellationToken);

            if (userProfile == null) {
                throw new AppException("User profile not found", null, StatusCodes.Status404NotFound);
            }

            string imageUploadToken = JwtHelper.GenerateJWT(
                _config.SecretKey,
                SecurityAlgorithms.HmacSha256Signature,
                (descriptor) => {
                    descriptor.Issuer = _config.Issuer;
                    descriptor.Expires = DateTime.UtcNow + TimeSpan.FromMinutes(_config.ExpireMinutes);
                    descriptor.Subject = new ClaimsIdentity(new List<Claim> {
                        new Claim("user_id", request.UserId),
                        new Claim("image_id", Guid.NewGuid().ToString()),
                        new Claim("category", Categories.UserUploadedBanner),
                        new Claim("max_size", _config.MaxBannberSize.ToString())
                    });
                });

            return new GetBannerUploadTokenResponseDto {
                UploadToken = imageUploadToken
            };
        }

    }
}
