using Application.Handlers;
using AutoMapper;
using Search.API.Application.DtoModels;
using Search.Infrastructure.Contracts;

namespace Search.API.Application.Queries.Handlers {
    public class SearchVideosByTagsQueryHandler : IQueryHandler<SearchVideosByTagsQuery, SearchResponseDto> {

        private readonly IVideosQueryManager _queryManager;
        private readonly IMapper _mapper;

        public SearchVideosByTagsQueryHandler (IVideosQueryManager queryManager, IMapper mapper) {
            _queryManager = queryManager;
            _mapper = mapper;
        }

        public async Task<SearchResponseDto> Handle (SearchVideosByTagsQuery request, CancellationToken cancellationToken) {
            var (totalCount, videos) = await _queryManager.SearchVideosByTags(request.Tags, request.Pagination, cancellationToken);

            return new SearchResponseDto {
                TotalCount = totalCount,
                Items = _mapper.Map<List<VideoDto>>(videos, options => options.Items["resolveUrl"] = true).OfType<object>().ToList()
            };
        }

    }
}
