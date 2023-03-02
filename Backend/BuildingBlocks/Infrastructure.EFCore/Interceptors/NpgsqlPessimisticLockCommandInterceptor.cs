using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Infrastructure.EFCore.Interceptors {
    public class NpgsqlPessimisticLockCommandInterceptor : DbCommandInterceptor {

        public override InterceptionResult<DbDataReader> ReaderExecuting (DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result) {
            ManipulateCommand(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync (DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default) {
            ManipulateCommand(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        private static void ManipulateCommand (DbCommand command) {
            if (command.Transaction != null) {
                if (command.CommandText.StartsWith("-- FOR UPDATE SKIP LOCKED", StringComparison.Ordinal)) {
                    command.CommandText += " FOR UPDATE SKIP LOCKED";
                } else if (command.CommandText.StartsWith("-- FOR UPDATE", StringComparison.Ordinal)) {
                    command.CommandText += " FOR UPDATE";
                }
            }
        }

    }
}
