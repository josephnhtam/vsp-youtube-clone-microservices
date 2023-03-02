using Application.Handlers;
using AutoMapper;
using Library.Domain.Contracts;
using Library.Domain.Models;

namespace Library.API.Application.Queries.Handlers {
    public class GetPlaylistRefQueryHandler : IQueryHandler<GetPlaylistRefQuery, PlaylistRef?> {

        private readonly IPlaylistRefRepository _repository;
        private readonly IMapper _mapper;

        public GetPlaylistRefQueryHandler (IPlaylistRefRepository repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PlaylistRef?> Handle (GetPlaylistRefQuery request, CancellationToken cancellationToken) {
            var result = await _repository.GetPlaylistRefAsync(request.UserId, request.PlaylistId, cancellationToken);
            if (result == null) return null;
            return _mapper.Map<PlaylistRef>(result);
        }
    }
}
