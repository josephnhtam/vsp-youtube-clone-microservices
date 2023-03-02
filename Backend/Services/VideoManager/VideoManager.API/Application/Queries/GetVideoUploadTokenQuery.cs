using Application.Contracts;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.Queries {
    public class GetVideoUploadTokenQuery : IQuery<VideoUploadTokenResponseDto> {
        public string CreatorId { get; set; }
        public Guid VideoId { get; set; }

        public GetVideoUploadTokenQuery (string creator, Guid videoId) {
            CreatorId = creator;
            VideoId = videoId;
        }
    }
}
