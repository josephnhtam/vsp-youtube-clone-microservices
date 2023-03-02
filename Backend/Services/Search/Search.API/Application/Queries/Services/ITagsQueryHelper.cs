namespace Search.API.Application.Queries.Services {
    public interface ITagsQueryHelper {
        Task<List<string>> SearchTrendingTagsAsync (int maxTagsCount, CancellationToken cancellationToken = default);
        Task<List<string>> SearchRelevantTagsAsync (List<string> tags, int maxTagsCount, CancellationToken cancellationToken = default);
    }
}