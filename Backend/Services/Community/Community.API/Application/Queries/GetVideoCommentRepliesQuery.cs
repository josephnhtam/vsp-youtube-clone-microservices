using Application.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Queries {
    public class GetVideoCommentRepliesQuery : IQuery<List<VideoComment>> {
        public long CommentId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public GetVideoCommentRepliesQuery (long commentId, int page, int pageSize) {
            CommentId = commentId;
            Page = page;
            PageSize = pageSize;
        }
    }
}
