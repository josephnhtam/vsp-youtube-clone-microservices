using Elasticsearch.Net;
using Infrastructure.Elasticsearch;
using Microsoft.Extensions.Options;
using Nest;
using Polly;
using Search.Domain.Models;
using Search.Infrastructure.Configurations;
using Search.Infrastructure.Contracts;

namespace Search.Infrastructure.Managers {
    public class VideosCommandManager : IVideosCommandManager {

        private readonly IElasticClient _client;
        private readonly UpdateConfiguration _updateConfig;

        public VideosCommandManager (IElasticClient client, IOptions<UpdateConfiguration> updateConfig) {
            _client = client;
            _updateConfig = updateConfig.Value;
        }

        public async Task DeleteVideoAsync (Guid videoId, long version, CancellationToken cancellationToken = default) {
            await ElasticsearchHelper.ExecuteAsync(async () => {
                var getVideo = await _client.GetAsync<Video>(
                    videoId,
                    get => get.Index("videos").SourceIncludes(p => p.Version));

                if (getVideo.Found) {
                    if (getVideo.Source.Version < version) {
                        await _client.IndexAsync<object>(
                            new EmptyItem { Id = videoId.ToString(), IsDeleted = true, Version = version },
                            i => i
                               .Index("videos")
                               .OpType(OpType.Index)
                               .IfPrimaryTerm(getVideo.PrimaryTerm)
                               .IfSequenceNumber(getVideo.SequenceNumber),
                           cancellationToken
                       );
                    }
                }
            });
        }

        public async Task IndexVideoAsync (Video video, long version, CancellationToken cancellationToken = default) {
            object videoObj = video.IsDeleted ?
                new EmptyItem { Id = video.Id, Version = version, IsDeleted = true } :
                video;

            async Task IndexDocumentAsync (object videoObj, long? primaryTerm, long? sequenceNumber) {
                await _client.IndexAsync<object>(videoObj, i => i
                    .Index("videos")
                    .OpType(OpType.Index)
                    .IfPrimaryTerm(primaryTerm)
                    .IfSequenceNumber(sequenceNumber),
                    cancellationToken
                );
            }

            await ElasticsearchHelper.ExecuteAsync(async () => {
                var getVideo = await _client.GetAsync<Video>(
                    video.Id,
                    get => get.Index("videos").SourceIncludes(p => p.Version));

                if (getVideo.Found) {
                    if (getVideo.Source.Version < version) {
                        await IndexDocumentAsync(videoObj, getVideo.PrimaryTerm, getVideo.SequenceNumber);
                    }
                } else {
                    await IndexDocumentAsync(videoObj, null, null);
                }
            });
        }

        public async Task UpdateVideoVotesMetricsAsync (Guid videoId, long likesCount, long dislikesCount, CancellationToken cancellationToken = default) {
            await _client.UpdateAsync<object>(videoId.ToString(), u => u
                .Index("videos")
                .Doc(new {
                    Metrics = new {
                        LikesCount = likesCount,
                        DislikesCount = dislikesCount
                    }
                })
           );
        }

        public async Task UpdateVideoViewsMetricsAsync (Guid videoId, long viewsCount, CancellationToken cancellationToken = default) {
            await _client.UpdateAsync<object>(videoId.ToString(), u => u
                .Index("videos")
                .Doc(new {
                    Metrics = new {
                        ViewsCount = viewsCount
                    }
                })
           );
        }

        public async Task UpdateUserProfileAsync (UserProfile userProfile, CancellationToken cancellationToken = default) {
            string script = @$"ctx._source.creatorProfile.displayName = params.displayName;" +
                            @$"ctx._source.creatorProfile.handle = params.handle;" +
                            @$"ctx._source.creatorProfile.thumbnailUrl = params.thumbnailUrl;" +
                            @$"ctx._source.creatorProfile.primaryVersion = params.primaryVersion;";

            var scriptParams = new Dictionary<string, object> {
                { "displayName", userProfile.DisplayName },
                { "handle", userProfile.Handle! },
                { "thumbnailUrl", userProfile.ThumbnailUrl! },
                { "primaryVersion", userProfile.PrimaryVersion },
            };

            var policy = Polly.Policy
                .HandleResult<UpdateByQueryResponse>((response) => response.VersionConflicts > 0)
                .WaitAndRetryAsync(_updateConfig.MaxVideoUpdateRetryCount,
                    retryAttempts => TimeSpan.FromSeconds(Math.Pow(2f, retryAttempts)));

            await policy.ExecuteAsync(async () => {
                return await _client.UpdateByQueryAsync<Video>(u => u
                    .Index("videos")
                    .Conflicts(Conflicts.Proceed)
                    .Query(q => q
                        .Term(t => t
                            .Field(p => p.CreatorProfile.Id)
                            .Value(userProfile.Id)
                        ) & q
                        .Range(r => r
                            .Field(p => p.CreatorProfile.PrimaryVersion)
                            .LessThan(userProfile.PrimaryVersion)
                        )
                    )
                    .Script(s => s
                        .Source(script)
                        .Params(scriptParams)
                    )
                );
            });
        }

    }
}
