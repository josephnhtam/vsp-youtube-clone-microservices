using Application.Contracts;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.Queries {
    public class GetVideosQuery : IQuery<GetVideosResponseDto> {
        public string UserId { get; set; }
        public GetVideosRequestDto Request { get; set; }

        public GetVideosQuery (string userId, GetVideosRequestDto request) {
            UserId = userId;
            Request = request;
        }
    }
}
