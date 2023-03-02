using Microsoft.Extensions.Options;
using Nest;
using Search.Domain.Models;
using Search.Infrastructure.Configurations;
using Search.Infrastructure.Contracts;
using Search.Infrastructure.Specifications;

namespace Search.Infrastructure.Managers {
    public class VideosQueryManager : IVideosQueryManager {

        private readonly IElasticClient _client;
        private readonly ScoreBoostConfiguration _config;

        public VideosQueryManager (IElasticClient client, IOptions<ScoreBoostConfiguration> config) {
            _client = client;
            _config = config.Value;
        }

        public async Task<List<string>> SearchSignificantTags (List<string>? tags, int? days, int sampleSize, int maxTagsCount, bool random, CancellationToken cancellationToken = default) {
            if (tags?.Count == 0 && !days.HasValue) {
                return new List<string>();
            }

            IAggregationContainer Aggregation (AggregationContainerDescriptor<Video> descriptor) {
                if (!random) {
                    return descriptor
                        .SignificantTerms("tags", t => t
                            .Field("tags.keyword")
                            .ShardSize(sampleSize)
                            .Size(maxTagsCount)
                        );
                } else {
                    return descriptor
                        .SignificantTerms("tags", t => t
                            .Field("tags.keyword")
                            .ShardSize(sampleSize)
                            .Aggregations(a => a
                                .BucketScript("random_score", b => b
                                    .BucketsPath(p => p
                                        .Add("count", "_count")
                                    )
                                    .Script("Math.random()")
                                )
                                .BucketSort("sort", bs => bs
                                    .Sort(s => s
                                        .Descending("random_score")
                                    )
                                )
                            )
                        );
                }
            }

            IBoolQuery BoolQuery (BoolQueryDescriptor<Video> descriptor) {
                var query = descriptor;

                IEnumerable<Func<QueryContainerDescriptor<Video>, QueryContainer>> CreateMust () {
                    if (tags != null && tags.Count > 0) {
                        yield return m => m
                            .Terms(t => t
                                .Field("tags.keyword")
                                .Terms(tags)
                            );
                    }

                    if (days != null) {
                        yield return m => m
                            .DateRange(d => d
                                .GreaterThan($"now-{days}d")
                            );
                    }
                }

                var must = CreateMust().ToArray();
                if (must.Length > 0) {
                    query = query.Must(must);
                }

                return query;
            }

            var result = await _client.SearchAsync<Video>(s => s
                .Index("videos")
                .Size(0)
                .Query(q => q.Bool(BoolQuery))
                .Aggregations(Aggregation),
                cancellationToken
            );

            return result.Aggregations.SignificantTerms("tags").Buckets.Select(x => x.Key).ToList();
        }

        public async Task<List<string>> SearchPopularTags (int sampleSize, int maxTagsCount, float randomness = 1, CancellationToken cancellationToken = default) {
            randomness = Math.Clamp(randomness, 0f, 1f);

            string script;
            if (randomness >= 1f) {
                script = "params.count * Math.random()";
            } else {
                script = $"params.count * ({1f - randomness} + (Math.random() * {randomness}))";
            }

            var result = await _client.SearchAsync<Video>(s => s
                .Index("videos")
                .Size(0)
                .Aggregations(ta => ta
                    .Terms("tags", t => t
                        .Field("tags.keyword")
                        .Size(sampleSize)
                        .Order(o => o.CountDescending())
                        .Aggregations(sa => sa
                            .BucketScript("final_score", b => b
                                .BucketsPath(p => p
                                    .Add("count", "_count")
                                )
                                .Script(script)
                            )
                            .BucketSort("sort", b => b
                                .Sort(s => s
                                    .Descending("final_score")
                                )
                                .Size(maxTagsCount)
                            )
                        )
                    )
                ),
                cancellationToken
            );

            return result.Aggregations.Terms("tags").Buckets.Select(x => x.Key).ToList();
        }

        public async Task<(long, List<Video>)> SearchVideosByTags (List<string> tags, Pagination pagination, CancellationToken cancellationToken = default) {
            if (tags.Count == 0) {
                return (0, new List<Video>());
            }

            var config = _config.SearchVideoByTagsConfig;

            IBoolQuery BoolQuery (BoolQueryDescriptor<Video> descriptor) {
                var query = descriptor;

                if (tags.Count > 0) {
                    query = query.Must(m => m
                        .Terms(t => t
                            .Field("tags.keyword")
                            .Terms(tags)
                            .Boost(config.SearchByTagsBoost)
                        )
                    );

                    query = query.Should(s => s
                        .Match(m => m
                            .Field(f => f.Tags)
                            .Query(string.Join(" ", tags))
                            .Boost(config.TagsMatchBoost)
                        ), s => s
                        .DistanceFeature(d => d
                            .Field(f => f.CreateDate)
                            .Origin(DateMath.Now)
                            .Pivot(new Time(TimeSpan.FromDays(config.TimeDistasnceBoost.TimePivotDays)))
                            .Boost(config.TimeDistasnceBoost.TimeDistanceBoost)
                        )
                    );
                }

                return query;
            }

            var result = await _client.SearchAsync<Video>(s => s
                .Index("videos")
                .From(Math.Max(0, (pagination.Page - 1) * pagination.PageSize))
                .Size(pagination.PageSize)
                .Query(q => q
                    .FunctionScore(fs => fs
                        .Query(fq => fq
                            .Bool(BoolQuery)
                        )
                        .Functions(f => f
                            .ScriptScore(ss => ss
                                .Script(s => s
                                    .Source(MetricsScore(config.MetricsBoost))
                                )
                            )
                        )
                    )
                ),
                cancellationToken
            );

            return (result.HitsMetadata.Total.Value, result.Hits.Select(h => h.Source).ToList());
        }

        public async Task<(long, List<Video>)> SearchVideos (VideoSearchParameters searchParams, CancellationToken cancellationToken = default) {
            var config = _config.SearchVideoConfig;

            var result = await _client.SearchAsync<Video>(s => Search(s, searchParams, config), cancellationToken);

            return (result.HitsMetadata.Total.Value, result.Hits.Select(h => h.Source).ToList());
        }

        private ISearchRequest Search (SearchDescriptor<Video> descriptor, VideoSearchParameters searchParams, SearchVideoBoostConfiguration config) {
            var search = descriptor
                .Index("videos")
                .From(Math.Max(0, (searchParams.Pagination.Page - 1) * searchParams.Pagination.PageSize))
                .Size(searchParams.Pagination.PageSize)
                .Query(descriptor => Query(descriptor, searchParams, config));

            switch (searchParams.Sort) {
                case VideoSort.CreateDate:
                    search = search.Sort(s => s
                        .Descending(p => p.CreateDate)
                    );
                    break;
                case VideoSort.ViewsCount:
                    search = search.Sort(s => s
                        .Descending(p => p.Metrics.ViewsCount)
                    );
                    break;
                case VideoSort.LikesCount:
                    search = search.Sort(s => s
                        .Descending(p => p.Metrics.LikesCount)
                    );
                    break;
            }

            return search;
        }

        private QueryContainer Query (QueryContainerDescriptor<Video> descriptor, VideoSearchParameters searchParams, SearchVideoBoostConfiguration config) {
            QueryContainer query;

            if (searchParams.Sort == VideoSort.Relevance) {
                query = descriptor
                    .FunctionScore(fs => fs
                        .Query(q => q
                            .Bool(b => BoolQuery(b, searchParams, config))
                        )
                        .Functions(f => f
                            .ScriptScore(ss => ss
                                .Script(s => s
                                    .Source(MetricsScore(config.MetricsBoost))
                                )
                            )
                        )
                    );
            } else {
                query = descriptor.Bool(b => BoolQuery(b, searchParams, config));
            }

            return query;
        }

        private static IBoolQuery BoolQuery (BoolQueryDescriptor<Video> descriptor, VideoSearchParameters searchParams, SearchVideoBoostConfiguration config) {
            var query = descriptor
                .Should(s => s
                    .DistanceFeature(d => d
                        .Field(f => f.CreateDate)
                        .Origin(DateMath.Now)
                        .Pivot(new Time(TimeSpan.FromDays(config.TimeDistasnceBoost.TimePivotDays)))
                        .Boost(config.TimeDistasnceBoost.TimeDistanceBoost)
                    )
                );

            if (searchParams.Query != null) {
                query = query
                    .Must(m => m
                        .MultiMatch(m => m
                            .Fields(new[] {
                                $"title^{config.TitleMatchBoost}",
                                $"title.*^{config.TitleMatchBoost}",
                                $"tags^{config.TagsMatchBoost}",
                                $"creatorProfile.displayName^{config.CreatorNameMatchBoost}" }
                            )
                            .Query(searchParams.Query)
                            .Fuzziness(Fuzziness.Auto)
                        )
                    );
            }

            IEnumerable<Func<QueryContainerDescriptor<Video>, QueryContainer>> CreateFilters () {
                if (searchParams.CreatorIds != null && searchParams.CreatorIds.Count() > 0) {
                    yield return f => f
                        .Terms(t => t
                            .Field(p => p.CreatorProfile.Id)
                            .Terms(searchParams.CreatorIds)
                        );
                }

                if (searchParams.PeriodRange?.From != null && searchParams.PeriodRange?.To != null) {
                    yield return f => f
                        .DateRange(d => d
                            .GreaterThan(searchParams.PeriodRange.From.Value.DateTime)
                            .LessThan(searchParams.PeriodRange.To.Value.DateTime)
                        );
                } else if (searchParams.PeriodRange?.From != null) {
                    yield return f => f
                        .DateRange(d => d
                            .GreaterThan(searchParams.PeriodRange.From.Value.DateTime)
                        );
                } else if (searchParams.PeriodRange?.To != null) {
                    yield return f => f
                        .DateRange(d => d
                            .LessThan(searchParams.PeriodRange.To.Value.DateTime)
                        );
                }
            }

            var filters = CreateFilters().ToArray();
            if (filters.Length > 0) {
                query = query.Filter(filters);
            }

            return query;
        }

        private string MetricsScore (VideoMetricsBoostConfiguration config) {
            string metricsScore =
                $"doc['metrics.viewsCount'].value * {config.ViewsCountFactor}" +
                $"+ doc['metrics.likesCount'].value * {config.LikesCountFactor}" +
                $"+ doc['metrics.dislikesCount'].value * {config.DislikesCountFactor}";

            return $"_score * (1.0 + Math.min({config.MaxMetricsBoost}, Math.log10(1.0 + {metricsScore})))";
        }
    }
}
