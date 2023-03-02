using History.Domain.Models;
using History.Infrastructure.Contracts;
using History.Infrastructure.Specifications;
using Nest;

namespace History.Infrastructure.Managers {
    public class UserHistoryQueryManager : IUserHistoryQueryManager {

        private readonly IElasticClient _client;

        public UserHistoryQueryManager (IElasticClient client) {
            _client = client;
        }

        public async Task<(long totalCount, List<(string Id, Guid videoId, DateTimeOffset date)>)> SearchUserWatchHistory
            (UserWatchHistorySearchParameters searchParams, CancellationToken cancellationToken = default) {
            IBoolQuery Bool (BoolQueryDescriptor<UserVideoHistory> descriptor) {
                var boolQuery = descriptor;

                boolQuery = boolQuery.Filter(f => f
                    .Term(t => t
                        .Field(f => f.UserId)
                        .Value(searchParams.UserId)
                    )
                );

                if (!string.IsNullOrEmpty(searchParams.Query)) {
                    boolQuery = boolQuery.Must(m => m
                        .MultiMatch(m => m
                            .Fields(new[] { "title", "title.*" })
                            .Query(searchParams.Query)
                            .Fuzziness(Fuzziness.Auto)
                        )
                    );
                }

                if (searchParams.PeriodRange?.From != null && searchParams.PeriodRange?.To != null) {
                    boolQuery = boolQuery.Filter(f => f
                        .DateRange(d => d
                            .GreaterThan(searchParams.PeriodRange.From.Value.DateTime)
                            .LessThan(searchParams.PeriodRange.To.Value.DateTime)
                        )
                    );
                } else if (searchParams.PeriodRange?.From != null) {
                    boolQuery = boolQuery.Filter(f => f
                        .DateRange(d => d
                            .GreaterThan(searchParams.PeriodRange.From.Value.DateTime)
                        )
                    );
                } else if (searchParams.PeriodRange?.To != null) {
                    boolQuery = boolQuery.Filter(f => f
                        .DateRange(d => d
                            .LessThan(searchParams.PeriodRange.To.Value.DateTime)
                        )
                    );
                }

                return boolQuery;
            }

            var result = await _client.SearchAsync<UserVideoHistory>(s => s
                .Index("user_video_history")
                .Source(so => so
                    .Includes(i => i
                        .Field(f => f.VideoId)
                        .Field(f => f.Date)
                    )
                )
                .Sort(so => so
                    .Descending(f => f.Date)
                )
                .Skip(Math.Max(0, (searchParams.Pagination.Page - 1) * searchParams.Pagination.PageSize))
                .Take(searchParams.Pagination.PageSize)
                .Query(q => q
                    .Bool(Bool)
                )
            );

            return (result.HitsMetadata.Total.Value, result.Hits.Select(h => (h.Id, h.Source.VideoId, h.Source.Date)).ToList());
        }
    }
}
