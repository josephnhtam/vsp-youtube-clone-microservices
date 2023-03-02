using Application.Handlers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Exceptions;
using SharedKernel.Utilities;
using System.Security.Claims;
using VideoManager.API.Application.Configurations;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Application.Utilities;
using VideoManager.Domain.Contracts;

namespace VideoManager.API.Application.Queries.Handlers {
    public class GetVideoUploadTokenQueryHandler : IQueryHandler<GetVideoUploadTokenQuery, VideoUploadTokenResponseDto> {

        private readonly VideoUploadConfiguration _config;
        private readonly IVideoRepository _repository;

        public GetVideoUploadTokenQueryHandler (IOptions<VideoUploadConfiguration> config, IVideoRepository repository) {
            _config = config.Value;
            _repository = repository;
        }

        public async Task<VideoUploadTokenResponseDto> Handle (GetVideoUploadTokenQuery request, CancellationToken cancellationToken) {
            var video = await _repository.GetVideoByIdAsync(request.VideoId);

            if (video == null) {
                throw new AppException($"Video {request.VideoId} not found", null, StatusCodes.Status404NotFound);
            }

            if (video.CreatorId != request.CreatorId) {
                throw new AppException("Incorrect creator", null, StatusCodes.Status403Forbidden);
            }

            if (video.ProcessingStatus != Domain.Models.VideoProcessingStatus.WaitingForUserUpload) {
                throw new AppException("Video is already uploaded", null, StatusCodes.Status400BadRequest);
            }

            string videoUploadToken = JwtHelper.GenerateJWT(
                _config.SecretKey,
                SecurityAlgorithms.HmacSha256Signature,
                (descriptor) => {
                    descriptor.Issuer = _config.Issuer;
                    descriptor.Expires = DateTime.UtcNow + TimeSpan.FromMinutes(_config.ExpireMinutes);
                    descriptor.Subject = new ClaimsIdentity(new List<Claim> {
                        new Claim("video_id", request.VideoId.ToString()),
                        new Claim("category", Categories.UserUploadedRawVideo),
                        new Claim("creator_id", request.CreatorId)
                    });
                });

            return new VideoUploadTokenResponseDto {
                VideoUploadToken = videoUploadToken
            };
        }

    }
}
