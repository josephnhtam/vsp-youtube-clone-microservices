using Search.Domain.Models;

namespace Search.Infrastructure.Contracts {
    public interface IVideosCommandManager {
        Task IndexVideoAsync (Video video, long version, CancellationToken cancellationToken = default);
        Task UpdateVideoVotesMetricsAsync (Guid videoId, long likesCount, long dislikesCount, CancellationToken cancellationToken = default);
        Task UpdateVideoViewsMetricsAsync (Guid videoId, long viewsCount, CancellationToken cancellationToken = default);
        Task DeleteVideoAsync (Guid videoId, long version, CancellationToken cancellationToken = default);
        Task UpdateUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default);
    }
}
