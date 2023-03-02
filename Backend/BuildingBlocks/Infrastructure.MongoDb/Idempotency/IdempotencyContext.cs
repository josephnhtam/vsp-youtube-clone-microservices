using Infrastructure.Idempotency;
using Infrastructure.MongoDb.Contexts;
using MongoDB.Driver;

namespace Infrastructure.MongoDb.Idempotency {
    public class IdempotencyContext : IIdempotencyContext {

        private IMongoCollectionContext<IdempotentOperation> _context;

        public IdempotencyContext (IMongoCollectionContext<IdempotentOperation> context) {
            _context = context;
        }

        public async Task<bool> IsOperationIdStoredAsync (string operationId, CancellationToken cancellationToken = default) {
            var filter = Builders<IdempotentOperation>.Filter.Eq(x => x.Id, operationId);
            return (await _context.Collection.Find(filter).CountDocumentsAsync(cancellationToken)) > 0;
        }

        public async Task<bool> StoreOperationIdAsync (string operationId, CancellationToken cancellationToken = default) {
            if (!_context.IsInTransaction) {
                throw new Exception("Transaction is not created yet");
            }

            var operation = IdempotentOperation.Create(operationId, DateTimeOffset.UtcNow);

            try {
                await _context.Collection.InsertOneAsync(_context.CurrentSession, operation, null, cancellationToken);
            } catch (MongoWriteException ex) {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey) {
                    return false;
                }

                throw ex;
            }

            return true;
        }

    }
}
