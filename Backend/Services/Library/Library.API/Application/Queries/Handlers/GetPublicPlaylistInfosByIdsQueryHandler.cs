using Application.Handlers;
using Library.API.Application.Configurations;
using Library.API.Application.DtoModels;
using Library.API.Application.Queries.Handlers.Services;
using Library.Domain.Contracts;
using Library.Domain.Models;
using Microsoft.Extensions.Options;

namespace Library.API.Application.Queries.Handlers {
    public class GetPublicPlaylistInfosByIdsQueryHandler : IQueryHandler<GetPublicPlaylistInfosByIdsQuery, List<PlaylistInfoDto>> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _repository;
        private readonly IDtoResolver _dtoResolver;
        private readonly PlaylistQueryConfiguration _config;

        public GetPublicPlaylistInfosByIdsQueryHandler (
            IPlaylistRepository<Playlist, OrderedPlaylistItem> repository,
            IDtoResolver dtoResolver,
            IOptions<PlaylistQueryConfiguration> config) {
            _repository = repository;
            _dtoResolver = dtoResolver;
            _config = config.Value;
        }

        public async Task<List<PlaylistInfoDto>> Handle (GetPublicPlaylistInfosByIdsQuery request, CancellationToken cancellationToken) {
            var result = await _repository.GetPlaylists(
                request.PlaylistIds,
                _config.NonAvailableVideosTolerance + 1,
                true,
                cancellationToken);

            var infos = await _dtoResolver.ResolvePlaylistInfoDtos(
                result,
                request.UserId,
                cancellationToken);

            return infos;
        }
    }
}
