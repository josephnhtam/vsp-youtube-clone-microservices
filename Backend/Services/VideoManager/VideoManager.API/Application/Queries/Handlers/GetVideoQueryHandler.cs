using Application.Handlers;
using SharedKernel.Exceptions;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Queries.Handlers {
    public class GetVideoQueryHandler : IQueryHandler<GetVideoQuery, Video> {

        private readonly IVideoRepository _repository;

        public GetVideoQueryHandler (IVideoRepository repository) {
            _repository = repository;
        }

        public async Task<Video> Handle (GetVideoQuery request, CancellationToken cancellationToken) {
            var video = await _repository.GetVideoByIdAsync(request.VideoId);

            if (video == null) {
                throw new AppException($"Video {request.VideoId} not found", null, StatusCodes.Status404NotFound);
            }

            return video;
        }

    }
}
