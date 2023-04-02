using History.Domain.Models;
using Infrastructure.Elasticsearch;
using Nest;

namespace History.Infrastructure.IndexCreators {
    public class UserVideoHistoryIndexCreator : IIndexCreator {

        public IndexName GetIndexName () {
            return "user_video_history";
        }

        public ICreateIndexRequest CreateIndexRequest (CreateIndexDescriptor descriptor) {
            return descriptor
                .Settings(s => s.Analysis(Analysis))
                .Map<UserVideoHistory>(MapDocument);
        }

        private AnalysisDescriptor Analysis (AnalysisDescriptor analysis) {
            return analysis.
                Normalizers(n => n
                    .Custom("keyword_normalizer", c => c
                        .Filters(new[] { "lowercase" })
                    )
                );
        }

        private TypeMappingDescriptor<UserVideoHistory> MapDocument (TypeMappingDescriptor<UserVideoHistory> map) {
            return map
                .AutoMap()
                .Dynamic(DynamicMapping.Strict)
                .Properties(ps => ps
                    .Keyword(k => k
                        .Name(p => p.UserId)
                        .Norms(false)
                    )
                    .Keyword(k => k
                        .Name(p => p.VideoId)
                        .Norms(false)
                    )
                    .Keyword(k => k
                        .Name(p => p.Type)
                    )
                    .Text(t => t
                        .Name(p => p.Title)
                        .Analyzer("english")
                    //.Fields(f => f
                    //    .Text(p => p
                    //        .Name("zh")
                    //        .Analyzer("ik_max_word")
                    //    )
                    //)
                    )
                    .Text(k => k
                        .Name(p => p.Tags)
                        .Fields(f => f
                            .Keyword(k => k
                                .Name("keyword")
                                .Normalizer("keyword_normalizer")
                                .IgnoreAbove(256)
                            )
                        )
                    )
                    .Number(n => n
                        .Name(p => p.LengthSeconds)
                        .Type(NumberType.Integer)
                    )
                    .Date(d => d
                        .Name(p => p.Date)
                    )
                );
        }

    }

}
