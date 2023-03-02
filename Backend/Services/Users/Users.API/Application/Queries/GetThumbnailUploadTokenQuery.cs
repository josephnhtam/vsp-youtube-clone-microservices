using Application.Contracts;
using Users.API.Application.DtoModels;

namespace Users.API.Application.Queries {
    public class GetThumbnailUploadTokenQuery : IQuery<GetThumbnailUploadTokenResponseDto> {
        public string UserId { get; set; }

        public GetThumbnailUploadTokenQuery (string userId) {
            UserId = userId;
        }
    }
}
