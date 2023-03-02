using Application.Contracts;
using Community.API.Application.DtoModels;
using Community.Domain.Specifications;

namespace Community.API.Application.Queries {
    public class GetVideoForumQuery : IQuery<GetVideoForumResponseDto> {
        public Guid VideoId { get; set; }
        public int PageSize { get; set; }
        public VideoCommentSort Sort { get; set; }
        public string? UserId { get; set; }

        public GetVideoForumQuery (Guid videoId, int pageSize, VideoCommentSort sort, string? userId) {
            VideoId = videoId;
            PageSize = pageSize;
            Sort = sort;
            UserId = userId;
        }
    }
}
