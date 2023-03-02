using Application.Handlers;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Infrastructure.Contracts;

namespace Library.API.Application.Queries.Handlers {
    public class GetVideosQueryHandler : IQueryHandler<GetVideosQuery, GetVideosResponseDto> {

        private readonly IVideoQueryHelper _queryHelper;
        private readonly IDtoResolver _dtoResolver;

        public GetVideosQueryHandler (IVideoQueryHelper queryHelper, IDtoResolver dtoResolver) {
            _queryHelper = queryHelper;
            _dtoResolver = dtoResolver;
        }

        public async Task<GetVideosResponseDto> Handle (GetVideosQuery request, CancellationToken cancellationToken) {
            var skip = Math.Max(0, request.Pagination.Page - 1) * request.Pagination.PageSize;
            var limit = request.Pagination.PageSize;

            var videos = await _queryHelper
                .GetVideosAsync(request.UserId, request.PublicOnly, request.Sort, skip, limit, cancellationToken);

            var response = new GetVideosResponseDto {
                TotalCount = videos.TotalCount,
                Videos = await _dtoResolver.ResolveVideoDtos(videos.Videos, cancellationToken)
            };

            return response;
        }

    }
}
