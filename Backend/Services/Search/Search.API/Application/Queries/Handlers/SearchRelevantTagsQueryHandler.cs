using Application.Handlers;
using Search.API.Application.Queries.Services;

namespace Search.API.Application.Queries.Handlers {
    public class SearchRelevantTagsQueryHandler : IQueryHandler<SearchRelevantTagsQuery, List<string>> {

        private readonly ITagsQueryHelper _queryHelper;

        public SearchRelevantTagsQueryHandler (ITagsQueryHelper queryHelper) {
            _queryHelper = queryHelper;
        }

        public async Task<List<string>> Handle (SearchRelevantTagsQuery request, CancellationToken cancellationToken) {
            var inputTags = request.Tags.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            return await _queryHelper.SearchRelevantTagsAsync(inputTags, request.MaxTagsCount, cancellationToken);
        }

    }
}
