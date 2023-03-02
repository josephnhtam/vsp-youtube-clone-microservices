using Application.Handlers;
using Search.API.Application.Queries.Services;

namespace Search.API.Application.Queries.Handlers {
    public class SearchTrendingTagsQueryHandler : IQueryHandler<SearchTrendingTagsQuery, List<string>> {

        private readonly ITagsQueryHelper _queryHelper;

        public SearchTrendingTagsQueryHandler (ITagsQueryHelper queryHelper) {
            _queryHelper = queryHelper;
        }

        public async Task<List<string>> Handle (SearchTrendingTagsQuery request, CancellationToken cancellationToken) {
            return await _queryHelper.SearchTrendingTagsAsync(request.MaxTagsCount, cancellationToken);
        }

    }
}
