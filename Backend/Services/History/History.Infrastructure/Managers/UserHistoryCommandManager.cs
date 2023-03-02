using Elasticsearch.Net;
using History.Domain.Models;
using History.Infrastructure.Contracts;
using Nest;

namespace History.Infrastructure.Managers {
    public class UserHistoryCommandManager : IUserHistoryCommandManager {

        private readonly IElasticClient _client;

        public UserHistoryCommandManager (IElasticClient client) {
            _client = client;
        }

        public async Task<bool> AddUserWatchHistory (string userId, Guid videoId, string title, string[] tags, int lengthSeconds, DateTimeOffset date, CancellationToken cancellationToken = default) {
            var getLastHistory = await _client.SearchAsync<UserVideoHistory>(s => s
                .Index("user_video_history")
                .Source(so => so
                    .Includes(i => i
                        .Field(f => f.Date)
                    )
                )
                .Sort(so => so
                    .Descending(f => f.Date)
                )
                .Take(1)
                .Query(q => q
                    .Bool(b => b
                        .Filter(f => f
                            .Term(t => t
                                .Field(f => f.UserId)
                                .Value(userId)
                            ) &
                            f.Term(t => t
                                .Field(f => f.VideoId)
                                .Value(videoId)
                            )
                        )
                    )
                )
            );

            var lastHistory = getLastHistory.Hits.FirstOrDefault()?.Source;
            if (lastHistory != null && (date.UtcDateTime - lastHistory.Date) < TimeSpan.FromDays(1)) {
                return false;
            }

            var watchHistory = new UserVideoHistory {
                Type = UserVideoHistoryType.Watch,
                UserId = userId,
                VideoId = videoId,
                Title = title,
                Tags = tags,
                LengthSeconds = lengthSeconds,
                Date = date.UtcDateTime
            };

            await _client.IndexAsync<UserVideoHistory>(watchHistory, i => i
                    .Index("user_video_history")
                    .OpType(OpType.Index),
                    cancellationToken
                );

            return true;
        }

        public async Task ClearUserWatchHistory (string userId, CancellationToken cancellationToken = default) {
            await _client.DeleteByQueryAsync<UserVideoHistory>(d => d
                .Index("user_video_history")
                .Conflicts(Conflicts.Proceed)
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.UserId)
                        .Value(userId)
                    )
                )
            );
        }

        public async Task RemoveVideoFromUserWatchHistory (string userId, Guid videoId, CancellationToken cancellationToken = default) {
            await _client.DeleteByQueryAsync<UserVideoHistory>(d => d
                .Index("user_video_history")
                .Conflicts(Conflicts.Proceed)
                .Query(q => q
                    .Bool(b => b
                        .Filter(f => f
                            .Term(t => t
                                .Field(f => f.UserId)
                                .Value(userId)
                            ) &
                            f.Term(t => t
                                .Field(f => f.VideoId)
                                .Value(videoId)
                            )
                        )
                    )
                )
            );
        }
    }
}
