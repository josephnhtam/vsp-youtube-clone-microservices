using Nest;
using Search.Domain.Models;

namespace Search.Infrastructure.IndexCreators {
    public class VideosIndexCreator : SearchableDocumentIndexCreator<Video> {

        public override IndexName GetIndexName () {
            return "videos";
        }

        protected override TypeMappingDescriptor<Video> MapDocument (TypeMappingDescriptor<Video> map) {
            return base.MapDocument(map)
                .Properties(ps => ps
                    .Keyword(k => k
                        .Name(p => p.ThumbnailUrl)
                        .Index(false)
                        .Norms(false)
                        .DocValues(false)
                    )
                    .Keyword(k => k
                        .Name(p => p.PreviewThumbnailUrl)
                        .Index(false)
                        .Norms(false)
                        .DocValues(false)
                    )
                    .Number(n => n
                        .Name(p => p.LengthSeconds)
                        .Type(NumberType.Integer)
                    )
                    .Text(n => n
                        .Name(p => p.Description)
                        .Index(false)
                        .Norms(false)
                    )
                    .Object<VideoMetrics>(o => o
                        .Name(p => p.Metrics)
                        .Properties(ops => ops
                            .Number(l => l
                                .Type(NumberType.Long)
                                .Name(p => p.ViewsCount)
                            )
                            .Number(l => l
                                .Type(NumberType.Long)
                                .Name(p => p.LikesCount)
                            )
                            .Number(l => l
                                .Type(NumberType.Long)
                                .Name(p => p.DislikesCount)
                            )
                        )
                    )
                    .Date(d => d
                        .Name(p => p.CreateDate)
                    )
                );
        }

    }
}
