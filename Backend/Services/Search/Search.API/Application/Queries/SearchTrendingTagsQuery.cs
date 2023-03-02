using Application.Contracts;

namespace Search.API.Application.Queries {
    public class SearchTrendingTagsQuery : IQuery<List<string>> {
        public int MaxTagsCount { get; set; }

        public SearchTrendingTagsQuery (int maxTagsCount) {
            MaxTagsCount = maxTagsCount;
        }
    }
}
