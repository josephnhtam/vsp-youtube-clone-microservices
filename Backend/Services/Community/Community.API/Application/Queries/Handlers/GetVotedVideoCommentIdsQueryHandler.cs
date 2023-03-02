using Application.Handlers;
using Community.Domain.Contracts;
using Community.Domain.Models;

namespace Community.API.Application.Queries.Handlers {
    public class GetVotedVideoCommentIdsQueryHandler : IQueryHandler<GetVotedVideoCommentIdsQuery, List<UserVideoCommentVote>> {

        private readonly IVideoCommentVoteRepository _repository;

        public GetVotedVideoCommentIdsQueryHandler (IVideoCommentVoteRepository repository) {
            _repository = repository;
        }

        public async Task<List<UserVideoCommentVote>> Handle (GetVotedVideoCommentIdsQuery request, CancellationToken cancellationToken) {
            return await _repository.GetVotedVideoCommentsAsync(request.VideoId, request.UserId);
        }

    }
}
