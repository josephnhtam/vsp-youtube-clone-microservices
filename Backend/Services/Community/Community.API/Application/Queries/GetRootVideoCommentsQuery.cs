using Application.Contracts;
using Community.Domain.Models;
using Community.Domain.Specifications;

namespace Community.API.Application.Queries {
    public class GetRootVideoCommentsQuery : IQuery<List<VideoComment>> {
        public Guid VideoId { get; set; }
        public DateTimeOffset? MaxDate { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public VideoCommentSort Sort { get; set; }

        public GetRootVideoCommentsQuery (Guid videoId, DateTimeOffset? maxDate, int page, int pageSize, VideoCommentSort sort) {
            VideoId = videoId;
            MaxDate = maxDate;
            Page = page;
            PageSize = pageSize;
            Sort = sort;
        }
    }
}
