using Application.Contracts;
using Search.API.Application.DtoModels;
using Search.Infrastructure.Specifications;

namespace Search.API.Application.Queries {
    public class SearchVideosByTagsQuery : IQuery<SearchResponseDto> {
        public List<string> Tags { get; set; }
        public Pagination Pagination { get; set; }

        public SearchVideosByTagsQuery (List<string> tags, Pagination pagination) {
            Tags = tags;
            Pagination = pagination;
        }
    }
}
