using Application.Handlers;
using Library.API.Application.Configurations;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Infrastructure.Contracts;
using Microsoft.Extensions.Options;

namespace Library.API.Application.Queries.Handlers {
    public class GetAllPlaylistInfosQueryHandler : IQueryHandler<GetAllPlaylistInfosQuery, GetPlaylistInfosResponseDto> {

        private readonly IDtoResolver _dtoResolver;
        private readonly IPlaylistQueryHelper _queryHelper;
        private readonly PlaylistQueryConfiguration _config;

        public GetAllPlaylistInfosQueryHandler (
            IDtoResolver dtoResolver,
            IPlaylistQueryHelper playlistQueryHelper,
            IOptions<PlaylistQueryConfiguration> config) {
            _dtoResolver = dtoResolver;
            _queryHelper = playlistQueryHelper;
            _config = config.Value;
        }

        public async Task<GetPlaylistInfosResponseDto> Handle (GetAllPlaylistInfosQuery request, CancellationToken cancellationToken) {
            var result = await _queryHelper.GetPlaylistsIncludingRefAsync(
                request.UserId,
                _config.NonAvailableVideosTolerance + 1,
                request.Pagination,
                cancellationToken);

            var infos = await _dtoResolver.ResolvePlaylistInfoDtos(
                result.Playlists,
                request.UserId,
                cancellationToken);

            return new GetPlaylistInfosResponseDto {
                TotalCount = result.TotalCount,
                Infos = infos
            };
        }
    }
}
