using Application.Contracts;
using Search.API.Application.DtoModels;
using Search.Infrastructure.Specifications;

namespace Search.API.Application.Queries {
    public class SearchVideosQuery : IQuery<SearchResponseDto> {
        public VideoSearchParameters SearchParams { get; set; }

        public SearchVideosQuery (VideoSearchParameters searchParams) {
            SearchParams = searchParams;
        }
    }
}
