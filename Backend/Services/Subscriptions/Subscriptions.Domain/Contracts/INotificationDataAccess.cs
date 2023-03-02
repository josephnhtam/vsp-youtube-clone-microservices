using Subscriptions.Domain.Models;

namespace Subscriptions.Domain.Contracts {
    public interface INotificationDataAccess {
        Task<bool> HasMessageAsync (string messageId);
        Task<int> GetUnreadMessageCountAsync (string userId);
        Task<string?> GetLastReceivedMessageIdAsync (string userId);
        Task<long> GetMessagesCountAsync (string userId, TimeSpan? historyExpirationTime);
        Task<(List<NotificationMessageWithChecked> messages, long totalCount)> GetMessagesAsync (string userId, int? page, int? pageSize, TimeSpan? historyExpirationTime);
        Task AddMessageAsync (NotificationMessage message, TimeSpan? expirationTime);
        Task AddMessageToUsersAsync (List<string> userIds, string messageId, DateTimeOffset now, TimeSpan? historyExpirationTime, CancellationToken cancellationToken = default);
        Task ResetUnreadMessageCountAsync (string userId);
        Task RemoveMessageFromUserAsync (string userId, string messageId);
        Task MarkMessageCheckedAsync (string userId, string messageId);
    }
}
