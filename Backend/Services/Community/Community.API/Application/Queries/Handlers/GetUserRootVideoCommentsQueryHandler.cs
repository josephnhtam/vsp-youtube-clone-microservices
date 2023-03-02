using Application.Handlers;
using Community.Domain.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Queries.Handlers {
    public class GetUserRootVideoCommentsQueryHandler : IQueryHandler<GetUserRootVideoCommentsQuery, List<VideoComment>> {

        private readonly IVideoCommentRepository _repository;

        public GetUserRootVideoCommentsQueryHandler (IVideoCommentRepository repository) {
            _repository = repository;
        }

        public async Task<List<VideoComment>> Handle (GetUserRootVideoCommentsQuery request, CancellationToken cancellationToken) {
            return await _repository.GetUserRootVideoCommentsAsync(request.VideoId, request.UserId, request.MaxCount);
        }

    }
}
