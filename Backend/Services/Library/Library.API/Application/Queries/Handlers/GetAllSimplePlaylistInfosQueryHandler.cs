using Application.Handlers;
using AutoMapper;
using Library.API.Application.DtoModels;
using Library.Infrastructure.Contracts;

namespace Library.API.Application.Queries.Handlers {
    public class GetAllSimplePlaylistInfosQueryHandler : IQueryHandler<GetAllSimplePlaylistInfosQuery, GetSimplePlaylistInfosResponseDto> {

        private readonly IPlaylistQueryHelper _queryHelper;
        private readonly IMapper _mapper;

        public GetAllSimplePlaylistInfosQueryHandler (IPlaylistQueryHelper queryHelper, IMapper mapper) {
            _queryHelper = queryHelper;
            _mapper = mapper;
        }

        public async Task<GetSimplePlaylistInfosResponseDto> Handle (GetAllSimplePlaylistInfosQuery request, CancellationToken cancellationToken) {
            var result = await _queryHelper.GetPlaylistsIncludingRefAsync(request.UserId, 0, request.Pagination, cancellationToken);

            return new GetSimplePlaylistInfosResponseDto {
                TotalCount = result.TotalCount,
                Infos = _mapper.Map<List<SimplePlaylistInfoDto>>(result.Playlists)
            };
        }
    }
}
