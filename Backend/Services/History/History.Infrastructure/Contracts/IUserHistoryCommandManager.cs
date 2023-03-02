namespace History.Infrastructure.Contracts {
    public interface IUserHistoryCommandManager {
        Task<bool> AddUserWatchHistory (string userId, Guid videoId, string title, string[] tags, int lengthSeconds, DateTimeOffset date, CancellationToken cancellationToken = default);
        Task RemoveVideoFromUserWatchHistory (string userId, Guid videoId, CancellationToken cancellationToken = default);
        Task ClearUserWatchHistory (string userId, CancellationToken cancellationToken = default);
    }
}
