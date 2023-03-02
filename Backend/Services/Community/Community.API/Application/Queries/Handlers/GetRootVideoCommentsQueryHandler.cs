using Application.Handlers;
using Community.Domain.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Queries.Handlers {
    public class GetRootVideoCommentsQueryHandler : IQueryHandler<GetRootVideoCommentsQuery, List<VideoComment>> {

        private readonly IVideoCommentRepository _repository;

        public GetRootVideoCommentsQueryHandler (IVideoCommentRepository repository) {
            _repository = repository;
        }

        public async Task<List<VideoComment>> Handle (GetRootVideoCommentsQuery request, CancellationToken cancellationToken) {
            return await _repository.GetRootVideoCommentsAsync(request.VideoId, request.MaxDate, request.Page, request.PageSize, request.Sort);
        }

    }
}
