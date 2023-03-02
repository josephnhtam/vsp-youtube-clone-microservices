using MongoDB.Driver;

namespace Infrastructure.MongoDb.Contexts {
    public interface IMongoClientContext : IDisposable {
        IMongoClient MongoClient { get; }
        IClientSessionHandle? CurrentSession { get; }
        bool IsInTransaction { get; }
        Task<IClientSessionHandle> StartSessionAsync (ClientSessionOptions? options = null, CancellationToken cancellationToken = default);
        Task<long> CommitAsync (CancellationToken cancellationToken = default);
        Task ExecuteTransactionAsync (Func<Task> task, TransactionOptions? options = null, CancellationToken cancellationToken = default);
        IMongoCollectionContext<TDocument> GetCollection<TDocument> ();
        void Reset ();
    }
}
