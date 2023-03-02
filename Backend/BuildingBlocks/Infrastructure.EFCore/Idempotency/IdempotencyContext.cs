using Infrastructure.EFCore.Exceptions;
using Infrastructure.EFCore.Utilities;
using Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System.Data.Common;

namespace Infrastructure.EFCore.Idempotency {
    public class IdempotencyContext<TDbContext> : IIdempotencyContext where TDbContext : DbContext {

        private readonly TDbContext _dbContext;

        public IdempotencyContext (TDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<bool> IsOperationIdStoredAsync (string operationId, CancellationToken cancellationToken = default) {
            return await _dbContext.Set<IdempotentOperation>().AnyAsync(x => x.Id == operationId, cancellationToken);
        }

        public async Task<bool> StoreOperationIdAsync (string operationId, CancellationToken cancellationToken = default) {
            if (_dbContext.Database.CurrentTransaction == null) {
                throw new Exception("Transaction is not created yet");
            }

            var operation = IdempotentOperation.Create(operationId, DateTimeOffset.UtcNow);

            try {
                await _dbContext.Set<IdempotentOperation>().AddAsync(operation, cancellationToken);
                await _dbContext.SaveChangesAsync();
            } catch (DbUpdateException ex) {
                if (ex.Entries.Any(x => x.Entity == operation)) {
                    var dbException = ex.FindInnerException<DbException>();

                    if (dbException != null && dbException.IsSqlState(SqlState.UniqueViolation)) {
                        return false;
                    }
                }

                throw ex;
            }

            return true;
        }

    }
}
