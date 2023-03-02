using Application.Contracts;

namespace Search.API.Application.Queries {
    public class SearchRelevantTagsQuery : IQuery<List<string>> {
        public string Tags { get; set; }
        public int MaxTagsCount { get; set; }

        public SearchRelevantTagsQuery (string tags, int maxTagsCount) {
            Tags = tags;
            MaxTagsCount = maxTagsCount;
        }
    }
}
