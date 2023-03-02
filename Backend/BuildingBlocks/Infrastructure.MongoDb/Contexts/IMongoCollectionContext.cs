using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.MongoDb.Contexts {
    public interface IMongoCollectionContextBase {
        IMongoClient MongoClient { get; }
        IClientSessionHandle? CurrentSession { get; }
        bool IsInTransaction { get; }
        IMongoClientContext ClientContext { get; }
        int WriteOperationsCount { get; }
        Task<long> CommitAsync (CancellationToken cancellationToken = default);
        void Reset ();
    }

    /// <summary>
    /// Use IMongoCollectionContext<T> for the support of unit of work.
    /// When it comes to unit of work, all the write operations will be committed in a transactional batch.
    /// </summary>
    public interface IMongoCollectionContext<TDocument> : IMongoCollectionContextBase {
        IMongoCollection<TDocument> Collection { get; }
        void BulkWriteAsync (IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions? options = null);
        void DeleteMany (FilterDefinition<TDocument> filter, DeleteOptions? options = null);
        void DeleteOne (FilterDefinition<TDocument> filter, DeleteOptions? options = null);
        void InsertMany (IEnumerable<TDocument> documents, InsertManyOptions? options = null);
        void InsertOne (TDocument document, InsertOneOptions? options = null);
        void ReplaceOne (FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions? options = null);
        void UpdateMany (FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null);
        void UpdateOne (Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null);
        void UpdateOne (FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null);
        Task<TDocument> FindOneAndLockAsync (Expression<Func<TDocument, bool>> filter, FindOneAndUpdateOptions<TDocument>? options = null, CancellationToken cancellationToken = default);
        Task<TDocument> FindOneAndLockAsync (FilterDefinition<TDocument> filter, FindOneAndUpdateOptions<TDocument>? options = null, CancellationToken cancellationToken = default);
        Task<TProjection> FindOneAndLockAsync<TProjection> (FilterDefinition<TDocument> filter, FindOneAndUpdateOptions<TDocument, TProjection>? options = null, CancellationToken cancellationToken = default);
    }
}
