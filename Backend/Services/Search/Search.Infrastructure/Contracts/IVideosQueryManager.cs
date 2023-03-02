using Search.Domain.Models;
using Search.Infrastructure.Specifications;

namespace Search.Infrastructure.Contracts {
    public interface IVideosQueryManager {
        Task<List<string>> SearchSignificantTags (List<string>? tags, int? days, int sampleSize, int maxTagsCount, bool random, CancellationToken cancellationToken = default);
        Task<List<string>> SearchPopularTags (int sampleSize, int maxTagsCount, float randomness = 1f, CancellationToken cancellationToken = default);
        Task<(long, List<Video>)> SearchVideosByTags (List<string> tags, Pagination pagination, CancellationToken cancellationToken = default);
        Task<(long, List<Video>)> SearchVideos (VideoSearchParameters searchParams, CancellationToken cancellationToken = default);
    }
}
