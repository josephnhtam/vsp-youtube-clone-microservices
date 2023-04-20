using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.EFCore {
    public static class ResilientTransactionExtensions {

        public static async Task ExecuteResilentTransaction (this DbContext dbContext, Func<Task> task, IsolationLevel? isolationLevel = null, CancellationToken cancellationToken = default) {
            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () => {
                foreach (var entry in dbContext.ChangeTracker.Entries()) {
                    entry.State = EntityState.Detached;
                }

                using var transaction = await (isolationLevel.HasValue ?
                        dbContext.Database.BeginTransactionAsync(isolationLevel.Value, cancellationToken) :
                        dbContext.Database.BeginTransactionAsync(cancellationToken));

                try {
                    await task.Invoke();
                    await transaction.CommitAsync();
                } catch (Exception) {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

    }
}
