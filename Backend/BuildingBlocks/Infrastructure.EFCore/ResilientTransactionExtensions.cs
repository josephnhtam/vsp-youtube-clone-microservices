using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.EFCore {
    public static class ResilientTransactionExtensions {

        public static async Task ExecuteResilientTransaction (this DbContext dbContext, Func<Task> task, IsolationLevel? isolationLevel = null, bool resetContext = true, CancellationToken cancellationToken = default) {
            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () => {
                if (resetContext) {
                    dbContext.ChangeTracker.Clear();
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
