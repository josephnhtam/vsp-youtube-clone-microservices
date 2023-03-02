using Application.Contracts;
using History.API.Application.DtoModels;
using History.Infrastructure.Specifications;

namespace History.API.Application.Queries {
    public class SearchUserWatchHistoryQuery : IQuery<SearchResponseDto> {
        public UserWatchHistorySearchParameters SearchParams { get; set; }

        public SearchUserWatchHistoryQuery (UserWatchHistorySearchParameters searchParams) {
            SearchParams = searchParams;
        }
    }
}
