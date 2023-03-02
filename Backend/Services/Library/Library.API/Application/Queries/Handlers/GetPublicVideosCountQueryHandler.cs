using Application.Handlers;
using Library.Domain.Contracts;

namespace Library.API.Application.Queries.Handlers {
    public class GetPublicVideosCountQueryHandler : IQueryHandler<GetPublicVideosCountQuery, int> {

        private readonly IVideoRepository _videoRepository;

        public GetPublicVideosCountQueryHandler (IVideoRepository videoRepository) {
            _videoRepository = videoRepository;
        }

        public async Task<int> Handle (GetPublicVideosCountQuery request, CancellationToken cancellationToken) {
            return await _videoRepository.GetVideosCount(request.UserId, true, cancellationToken);
        }
    }
}
