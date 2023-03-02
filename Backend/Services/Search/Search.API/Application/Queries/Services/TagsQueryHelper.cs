using Microsoft.Extensions.Options;
using Search.API.Application.Configurations;
using Search.Infrastructure.Contracts;

namespace Search.API.Application.Queries.Services {
    public class TagsQueryHelper : ITagsQueryHelper {

        private readonly IVideosQueryManager _queryManager;
        private readonly SearchConfiguration _config;

        public TagsQueryHelper (IVideosQueryManager queryManager, IOptions<SearchConfiguration> config) {
            _queryManager = queryManager;
            _config = config.Value;
        }

        public async Task<List<string>> SearchRelevantTagsAsync (List<string> inputTags, int maxTagsCount, CancellationToken cancellationToken = default) {
            if (inputTags.Count > 0) {
                var tags = await _queryManager.SearchSignificantTags(inputTags, null, _config.TagsSampleSize, maxTagsCount, true, cancellationToken);

                if (tags.Count < maxTagsCount) {
                    tags = tags.Union(inputTags).Take(maxTagsCount).ToList();
                }

                return tags;
            } else {
                return await SearchTrendingTagsAsync(maxTagsCount, cancellationToken);
            }
        }

        public async Task<List<string>> SearchTrendingTagsAsync (int maxTagsCount, CancellationToken cancellationToken = default) {
            var tags = await _queryManager.SearchSignificantTags(null, _config.TrendingSiginificantPeriodInDays, _config.TagsSampleSize, maxTagsCount, true, cancellationToken);

            if (tags.Count < maxTagsCount) {
                tags = tags
                    .Union(await _queryManager.SearchPopularTags(_config.TagsSampleSize, maxTagsCount - tags.Count, 1f, cancellationToken))
                    .ToList();
            }

            return tags;
        }

    }
}
