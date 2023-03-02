using History.Infrastructure.Specifications;

namespace History.Infrastructure.Contracts {
    public interface IUserHistoryQueryManager {
        Task<(long totalCount, List<(string Id, Guid videoId, DateTimeOffset date)>)> SearchUserWatchHistory
            (UserWatchHistorySearchParameters searchParams, CancellationToken cancellationToken = default);
    }
}
