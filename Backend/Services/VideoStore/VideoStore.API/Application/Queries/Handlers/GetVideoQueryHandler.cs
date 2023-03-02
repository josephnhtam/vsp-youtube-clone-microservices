using Application.Handlers;
using SharedKernel.Exceptions;
using VideoStore.Domain.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.Queries.Handlers {
    public class GetVideoQueryHandler : IQueryHandler<GetVideoQuery, Video> {

        private readonly IVideoRepository _repository;

        public GetVideoQueryHandler (IVideoRepository repository) {
            _repository = repository;
        }

        public async Task<Video> Handle (GetVideoQuery request, CancellationToken cancellationToken) {
            var video = await _repository.GetVideoByIdAsync(request.VideoId, cancellationToken);

            if (video == null) {
                throw new AppException($"Video {request.VideoId} not found", null, StatusCodes.Status404NotFound);
            }

            if ((video.Status != VideoStatus.Published || video.Visibility == VideoVisibility.Private) &&
                request.UserId != video.CreatorId) {
                throw new AppException("Unauthorized", null, StatusCodes.Status403Forbidden);
            }

            return video;
        }

    }
}
