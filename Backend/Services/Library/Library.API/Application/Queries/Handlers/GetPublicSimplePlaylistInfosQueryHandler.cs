using Application.Handlers;
using AutoMapper;
using Library.API.Application.DtoModels;
using Library.Domain.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Queries.Handlers {
    public class GetPublicSimplePlaylistInfosQueryHandler : IQueryHandler<GetPublicSimplePlaylistInfosByIdsQuery, List<SimplePlaylistInfoDto>> {

        private readonly IPlaylistRepository<Playlist, OrderedPlaylistItem> _repository;
        private readonly IMapper _mapper;

        public GetPublicSimplePlaylistInfosQueryHandler (IPlaylistRepository<Playlist, OrderedPlaylistItem> repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SimplePlaylistInfoDto>> Handle (GetPublicSimplePlaylistInfosByIdsQuery request, CancellationToken cancellationToken) {
            var result = await _repository.GetPlaylists(request.PlaylistIds, 0, true, cancellationToken);
            return _mapper.Map<List<SimplePlaylistInfoDto>>(result);
        }
    }
}
