using Infrastructure.Elasticsearch;
using Nest;
using Search.Domain.Models;

namespace Search.Infrastructure.IndexCreators {
    public abstract class SearchableDocumentIndexCreator<TDoc> : IIndexCreator where TDoc : SearchableItem {

        public abstract IndexName GetIndexName ();

        public virtual ICreateIndexRequest CreateIndexRequest (CreateIndexDescriptor descriptor) {
            return descriptor
                .Settings(s => s.Analysis(Analysis))
                .Map<TDoc>(MapDocument);
        }

        protected virtual AnalysisDescriptor Analysis (AnalysisDescriptor analysis) {
            return analysis.
                Normalizers(n => n
                    .Custom("keyword_normalizer", c => c
                        .Filters(new[] { "lowercase" })
                    )
                );
        }

        protected virtual TypeMappingDescriptor<TDoc> MapDocument (TypeMappingDescriptor<TDoc> map) {
            return map
                .AutoMap()
                .Dynamic(DynamicMapping.Strict)
                .Properties(ps => ps
                    .Keyword(k => k
                        .Name(p => p.Id)
                        .Norms(false)
                    )
                    .Object<UserProfile>(o => o
                        .Name(p => p.CreatorProfile)
                        .Properties(ops => ops
                            .Keyword(k => k
                                .Name(p => p.Id)
                            )
                            .Text(t => t
                                .Name(p => p.DisplayName)
                            )
                            .Keyword(k => k
                                .Name(p => p.Handle)
                            )
                            .Keyword(k => k
                                .Name(p => p.ThumbnailUrl)
                                .Index(false)
                                .Norms(false)
                                .DocValues(false)
                            )
                            .Number(n => n
                                .Name(p => p.PrimaryVersion)
                            )
                        )
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
                    .Text(t => t
                        .Name(p => p.Contents)
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
                    .Number(d => d
                        .Name(p => p.Version)
                        .Index(false)
                    )
                    .Boolean(b => b
                        .Name(p => p.IsDeleted)
                    )
                );
        }

    }
}
