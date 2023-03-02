using Nest;

namespace Infrastructure.Elasticsearch {
    public interface IIndexCreator {
        IndexName GetIndexName ();
        ICreateIndexRequest CreateIndexRequest (CreateIndexDescriptor descriptor);
    }
}
