using Application.Handlers;
using Library.API.Application.Configurations;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Infrastructure.Contracts;
using Microsoft.Extensions.Options;

namespace Library.API.Application.Queries.Handlers {
    public class GetCreatedPublicPlaylistInfosQueryHandler : IQueryHandler<GetCreatedPlaylistInfosQuery, GetPlaylistInfosResponseDto> {

        private readonly IDtoResolver _dtoResolver;
        private readonly IPlaylistQueryHelper _queryHelper;
        private readonly PlaylistQueryConfiguration _config;

        public GetCreatedPublicPlaylistInfosQueryHandler (
            IDtoResolver dtoResolver,
            IPlaylistQueryHelper playlistQueryHelper,
            IOptions<PlaylistQueryConfiguration> config) {
            _dtoResolver = dtoResolver;
            _queryHelper = playlistQueryHelper;
            _config = config.Value;
        }

        public async Task<GetPlaylistInfosResponseDto> Handle (GetCreatedPlaylistInfosQuery request, CancellationToken cancellationToken) {
            var result = await _queryHelper.GetPlaylistsAsync(
                request.UserId,
                request.PublicOnly,
                _config.NonAvailableVideosTolerance + 1,
                request.Pagination,
                cancellationToken);

            var infos = await _dtoResolver.ResolvePlaylistInfoDtos(
                result.Playlists,
                request.PublicOnly ? null : request.UserId,
                cancellationToken);

            return new GetPlaylistInfosResponseDto {
                TotalCount = result.TotalCount,
                Infos = infos
            };
        }
    }
}
