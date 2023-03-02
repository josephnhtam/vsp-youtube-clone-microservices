using Application.Contracts;
using Users.API.Application.DtoModels;

namespace Users.API.Application.Queries {
    public class GetBannerUploadTokenQuery : IQuery<GetBannerUploadTokenResponseDto> {
        public string UserId { get; set; }

        public GetBannerUploadTokenQuery (string userId) {
            UserId = userId;
        }
    }
}
