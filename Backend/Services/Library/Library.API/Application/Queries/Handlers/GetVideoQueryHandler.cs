using Application.Handlers;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Domain.Models;
using Library.Infrastructure.Contracts;

namespace Library.API.Application.Queries.Handlers {
    public class GetVideoQueryHandler : IQueryHandler<GetVideoQuery, VideoDto?> {

        private readonly ICachedVideoRepository _repository;
        private readonly IDtoResolver _dtoResolver;

        public GetVideoQueryHandler (ICachedVideoRepository repository, IDtoResolver dtoResolver) {
            _repository = repository;
            _dtoResolver = dtoResolver;
        }

        public async Task<VideoDto?> Handle (GetVideoQuery request, CancellationToken cancellationToken) {
            var video = await _repository.GetVideoByIdAsync(request.VideoId, cancellationToken);

            if (video == null || video.Visibility != VideoVisibility.Public) {
                return null;
            }

            var videoDto = await _dtoResolver.ResolveVideoDtos(new List<Video> { video }, cancellationToken);
            return videoDto.FirstOrDefault();
        }

    }
}
