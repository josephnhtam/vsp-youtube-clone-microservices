using Infrastructure.MongoDb.Configurations;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.MongoDb.Contexts {
    public sealed class MongoCollectionContext<TDocument> : IMongoCollectionContext<TDocument> {

        private readonly IMongoClientContext _clientContext;
        private readonly IMongoCollection<TDocument> _collection;
        private readonly List<MongoWriteOperation> _writeOperations;
        private readonly MongoDbContextConfiguration _config;

        public int WriteOperationsCount => _writeOperations.Count;
        public IMongoCollection<TDocument> Collection => _collection;

        public IMongoClient MongoClient => _clientContext.MongoClient;
        public IClientSessionHandle? CurrentSession => _clientContext.CurrentSession;
        public bool IsInTransaction => _clientContext.IsInTransaction;
        public IMongoClientContext ClientContext => _clientContext;

        public MongoCollectionContext (IMongoClientContext dbContext, IMongoCollection<TDocument> collection, IOptions<MongoDbContextConfiguration> config) {
            _clientContext = dbContext;
            _collection = collection;
            _config = config.Value;
            _writeOperations = new List<MongoWriteOperation>();
        }

        public void UpdateOne (Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                async () => (await _collection.UpdateOneAsync(filter, update, options)).ModifiedCount,
                async (session) => (await _collection.UpdateOneAsync(session, filter, update, options)).MatchedCount
            ));
        }

        public void UpdateOne (FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                async () => (await _collection.UpdateOneAsync(filter, update, options)).ModifiedCount,
                async (session) => (await _collection.UpdateOneAsync(session, filter, update, options)).ModifiedCount
            ));
        }

        public void UpdateMany (FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                 async () => (await _collection.UpdateManyAsync(filter, update, options)).ModifiedCount,
                 async (session) => (await _collection.UpdateManyAsync(session, filter, update, options)).ModifiedCount
             ));
        }

        public void ReplaceOne (FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                 async () => (await _collection.ReplaceOneAsync(filter, replacement, options)).ModifiedCount,
                 async (session) => (await _collection.ReplaceOneAsync(session, filter, replacement, options)).ModifiedCount
             ));
        }

        public void InsertOne (TDocument document, InsertOneOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                 async () => { await _collection.InsertOneAsync(document, options); return 1; },
                 async (session) => { await _collection.InsertOneAsync(session, document, options); return 1; }
             ));
        }

        public void InsertMany (IEnumerable<TDocument> documents, InsertManyOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                async () => { await _collection.InsertManyAsync(documents, options); return documents.Count(); },
                async (session) => { await _collection.InsertManyAsync(session, documents, options); return documents.Count(); }
            ));
        }

        public void DeleteOne (FilterDefinition<TDocument> filter, DeleteOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                async () => (await _collection.DeleteOneAsync(filter, options)).DeletedCount,
                async (session) => (await _collection.DeleteOneAsync(session, filter, options)).DeletedCount
            ));
        }

        public void DeleteMany (FilterDefinition<TDocument> filter, DeleteOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                async () => (await _collection.DeleteManyAsync(filter, options)).DeletedCount,
                async (session) => (await _collection.DeleteManyAsync(session, filter, options)).DeletedCount
            ));
        }

        public void BulkWriteAsync (IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions? options = null) {
            _writeOperations.Add(new MongoWriteOperation(
                async () => {
                    var result = await _collection.BulkWriteAsync(requests, options);
                    return result.InsertedCount + result.ModifiedCount + result.DeletedCount;
                },
                async (session) => {
                    var result = await _collection.BulkWriteAsync(session, requests, options);
                    return result.InsertedCount + result.ModifiedCount + result.DeletedCount;
                }
            ));
        }

        public async Task<TDocument> FindOneAndLockAsync (Expression<Func<TDocument, bool>> filter, FindOneAndUpdateOptions<TDocument>? options = null, CancellationToken cancellationToken = default) {
            if (!IsInTransaction) {
                throw new InvalidOperationException("Transaction is not created yet");
            }

            if (options == null) {
                options = new FindOneAndUpdateOptions<TDocument>();
            }

            options.BypassDocumentValidation = true;

            return await _collection.FindOneAndUpdateAsync(CurrentSession, filter,
                Builders<TDocument>.Update.Set("__concurrency_token", Guid.NewGuid().ToString()),
                options,
                cancellationToken);
        }

        public async Task<TDocument> FindOneAndLockAsync (FilterDefinition<TDocument> filter, FindOneAndUpdateOptions<TDocument>? options = null, CancellationToken cancellationToken = default) {
            if (!IsInTransaction) {
                throw new InvalidOperationException("Transaction is not created yet");
            }

            if (options == null) {
                options = new FindOneAndUpdateOptions<TDocument>();
            }

            options.BypassDocumentValidation = true;

            return await _collection.FindOneAndUpdateAsync(CurrentSession, filter,
                Builders<TDocument>.Update.Set("__concurrency_token", Guid.NewGuid().ToString()),
                options,
                cancellationToken);
        }

        public async Task<TProjection> FindOneAndLockAsync<TProjection> (FilterDefinition<TDocument> filter, FindOneAndUpdateOptions<TDocument, TProjection>? options = null, CancellationToken cancellationToken = default) {
            if (!IsInTransaction) {
                throw new InvalidOperationException("Transaction is not created yet");
            }

            if (options == null) {
                options = new FindOneAndUpdateOptions<TDocument, TProjection>();
            }

            options.BypassDocumentValidation = true;

            return await _collection.FindOneAndUpdateAsync(CurrentSession, filter,
                Builders<TDocument>.Update.Set("__concurrency_token", Guid.NewGuid().ToString()),
                options,
                cancellationToken);
        }

        public async Task<long> CommitAsync (CancellationToken cancellationToken = default) {
            List<MongoWriteOperation> writeOperations;

            lock (_writeOperations) {
                writeOperations = new List<MongoWriteOperation>(_writeOperations);
                _writeOperations.Clear();
            }

            long updateCount = 0;

            var session = _clientContext.CurrentSession;
            foreach (var writeOperation in writeOperations) {
                updateCount += await writeOperation.ExecuteAsync(session);
            }

            return updateCount;
        }

        public void Reset () {
            lock (_writeOperations) {
                _writeOperations.Clear();
            }
        }

        private class MongoWriteOperation {

            private readonly Func<Task<long>> _operation;
            private readonly Func<IClientSessionHandle, Task<long>> _operationWithSession;

            public MongoWriteOperation (Func<Task<long>> operation, Func<IClientSessionHandle, Task<long>> operationWithSession) {
                _operation = operation;
                _operationWithSession = operationWithSession;
            }

            public async Task<long> ExecuteAsync (IClientSessionHandle? session = null) {
                if (session != null) {
                    return await _operationWithSession.Invoke(session);
                } else {
                    return await _operation.Invoke();
                }
            }

        }
    }
}
