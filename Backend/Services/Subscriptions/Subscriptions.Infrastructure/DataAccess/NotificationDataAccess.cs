using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;
using SharedKernel.Utilities;
using StackExchange.Redis;
using Subscriptions.Domain.Contracts;
using Subscriptions.Domain.Models;
using System.Text.Json;

namespace Subscriptions.Infrastructure.DataAccess {
    public class NotificationDataAccess : INotificationDataAccess {

        private readonly IDatabase _database;
        private readonly ILogger<NotificationDataAccess> _logger;

        public NotificationDataAccess (IConnectionMultiplexer connection, ILogger<NotificationDataAccess> logger) {
            _database = connection.GetDatabase();
            _logger = logger;
        }

        public async Task<bool> HasMessageAsync (string messageId) {
            return await _database.KeyExistsAsync(messageId.ToString());
        }

        public async Task AddMessageAsync (NotificationMessage message, TimeSpan? expirationTime) {
            RedisValue messageJson = JsonSerializer.Serialize(message);
            await _database.StringSetAsync(GetMessageKey(message.Id.ToString()), messageJson, expirationTime);
        }

        public async Task AddMessageToUsersAsync (List<string> userIds, string msgId, DateTimeOffset now, TimeSpan? historyExpirationTime, CancellationToken cancellationToken = default) {
            RedisValue messageId = EncodeMessageId(msgId);
            var timestamp = now.UtcTicks;

            var tasks = new List<Task>();
            var batch = _database.CreateBatch();

            foreach (var userId in userIds) {
                tasks.Add(batch.SortedSetAddAsync(GetMessagesListKey(userId), messageId, timestamp));
                tasks.Add(batch.HashIncrementAsync(GetProfileKey(userId), unreadMessageCountKey, 1));
            }

            if (historyExpirationTime.HasValue) {
                var historyExpirationTimestamp = (now - historyExpirationTime.Value).UtcTicks;

                foreach (var userId in userIds) {
                    tasks.Add(batch.SortedSetRemoveRangeByScoreAsync(GetMessagesListKey(userId), 0, historyExpirationTimestamp));
                }
            }

            batch.Execute();

            await Task.WhenAll(tasks).WithCancellation(cancellationToken);
        }

        public async Task<string?> GetLastReceivedMessageIdAsync (string userId) {
            var result = await _database.HashGetAsync(GetProfileKey(userId), lastReceivedMessageIdKey);

            if (result.HasValue) {
                return result.ToString();
            } else {
                return null;
            }
        }

        public async Task<long> GetMessagesCountAsync (string userId, TimeSpan? historyExpirationTime) {
            if (historyExpirationTime.HasValue) {
                var historyExpirationTimestamp = (DateTimeOffset.UtcNow - historyExpirationTime.Value).UtcTicks;
                return await _database.SortedSetLengthAsync(GetMessagesListKey(userId), historyExpirationTimestamp);
            } else {
                return await _database.SortedSetLengthAsync(GetMessagesListKey(userId));
            }
        }

        public async Task<(List<NotificationMessageWithChecked> messages, long totalCount)> GetMessagesAsync (string userId, int? page, int? pageSize, TimeSpan? historyExpirationTime) {
            var key = GetMessagesListKey(userId);

            long skip = page.HasValue && pageSize.HasValue ? Math.Max(0, page.Value - 1) * pageSize.Value : 0;
            long take = pageSize.HasValue ? Math.Max(1, pageSize.Value) : -1;

            var batch = _database.CreateBatch();

            Task<RedisValue[]> messageIdValues;
            Task<long> totalCount;

            if (historyExpirationTime.HasValue) {
                var historyExpirationTimestamp = (DateTimeOffset.UtcNow - historyExpirationTime.Value).UtcTicks;

                messageIdValues = batch.SortedSetRangeByScoreAsync(
                    GetMessagesListKey(userId),
                    start: historyExpirationTimestamp,
                    order: Order.Descending,
                    skip: skip,
                    take: take
                );

                totalCount = batch.SortedSetLengthAsync(GetMessagesListKey(userId), historyExpirationTimestamp);
            } else {
                messageIdValues = batch.SortedSetRangeByScoreAsync(
                    GetMessagesListKey(userId),
                    order: Order.Descending,
                    skip: skip,
                    take: take
                );

                totalCount = batch.SortedSetLengthAsync(GetMessagesListKey(userId));
            }

            batch.Execute();

            await Task.WhenAll(messageIdValues, totalCount);

            List<(string messageId, bool isChecked)> messageIds = messageIdValues.Result.Select(DecodeMessageId).ToList();

            if (messageIds.Count == 0) {
                return (new List<NotificationMessageWithChecked>(), totalCount.Result);
            }

            var messageJsons = await _database.StringGetAsync(messageIds.Select(x => GetMessageKey(x.messageId)).ToArray());

            var messages = messageJsons.Select(json => {
                try {
                    return JsonSerializer.Deserialize<NotificationMessageWithChecked>(json.ToString());
                } catch (Exception ex) {
                    _logger.LogError(ex, "Failed to deserialize a message");
                    return null;
                }
            }).Where(x => x != null).ToDictionary(x => x.Id, x => x!);

            List<NotificationMessageWithChecked> results = messageIds.Select(x => {
                if (messages.TryGetValue(x.messageId, out var message)) {
                    message.Checked = x.isChecked;
                    return message;
                } else {
                    return null;
                }
            }).Where(x => x != null).ToList()!;

            return (results, totalCount.Result);
        }

        public async Task<int> GetUnreadMessageCountAsync (string userId) {
            var result = await _database.HashGetAsync(GetProfileKey(userId), unreadMessageCountKey);

            if (result.HasValue) {
                return (int)result;
            } else {
                return 0;
            }
        }

        public async Task ResetUnreadMessageCountAsync (string userId) {
            await _database.HashSetAsync(GetProfileKey(userId), new HashEntry[] {
                new HashEntry(unreadMessageCountKey, 0)
            });
        }

        public async Task RemoveMessageFromUserAsync (string userId, string messageId) {
            var key = GetMessagesListKey(userId);
            var tasks = new List<Task>();

            var batch = _database.CreateBatch();
            tasks.Add(_database.SortedSetRemoveAsync(key, EncodeMessageId(messageId)));
            tasks.Add(_database.SortedSetRemoveAsync(key, EncodeMessageId(messageId, true)));
            batch.Execute();

            await Task.WhenAll(tasks);
        }

        public async Task MarkMessageCheckedAsync (string userId, string messageId) {
            var key = GetMessagesListKey(userId);
            var tasks = new List<Task>();

            var timestamp = await _database.SortedSetScoreAsync(key, EncodeMessageId(messageId));

            if (!timestamp.HasValue) {
                throw new AppException("Message not found", null, StatusCodes.Status404NotFound);
            }

            var transaction = _database.CreateTransaction();

            tasks.Add(transaction.SortedSetRemoveAsync(key, EncodeMessageId(messageId)));
            tasks.Add(transaction.SortedSetAddAsync(key, EncodeMessageId(messageId, true), timestamp.Value));

            if (await transaction.ExecuteAsync()) {
                await Task.WhenAll(tasks);
            }
        }

        private static RedisKey GetProfileKey (string userId) {
            return $"profile-{userId}";
        }

        private static RedisKey GetMessagesListKey (string userId) {
            return $"msg-list-{userId}";
        }

        private static RedisKey GetMessageKey (string messageId) {
            return $"msg-{messageId}";
        }

        private static RedisValue EncodeMessageId (string messageId, bool isChecked = false) {
            if (isChecked) {
                return messageId.ToString() + "|c";
            } else {
                return messageId.ToString();
            }
        }

        private static (string messageId, bool isChecked) DecodeMessageId (RedisValue value) {
            var valueString = value.ToString();

            if (valueString.EndsWith("|c")) {
                return (valueString.Substring(0, valueString.Length - 2), true);
            } else {
                return (valueString, false);
            }
        }

        private const string unreadMessageCountKey = "unreadCount";
        private const string lastReceivedMessageIdKey = "lastMessageId";

    }
}
