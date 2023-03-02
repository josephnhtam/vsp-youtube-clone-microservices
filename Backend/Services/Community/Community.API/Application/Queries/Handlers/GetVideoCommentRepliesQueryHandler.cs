using Application.Handlers;
using Community.Domain.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Queries.Handlers {
    public class GetVideoCommentRepliesQueryHandler : IQueryHandler<GetVideoCommentRepliesQuery, List<VideoComment>> {

        private readonly IVideoCommentRepository _repository;

        public GetVideoCommentRepliesQueryHandler (IVideoCommentRepository repository) {
            _repository = repository;
        }

        public async Task<List<VideoComment>> Handle (GetVideoCommentRepliesQuery request, CancellationToken cancellationToken) {
            return await _repository.GetVideoCommentRepliesIdAsync(request.CommentId, request.Page, request.PageSize);
        }

    }
}
