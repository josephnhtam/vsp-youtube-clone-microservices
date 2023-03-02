using SharedKernel.Exceptions;
using System.Data.Common;

namespace Infrastructure.EFCore.Exceptions {
    public class TransientEFCoreExceptionIdentifier : IExceptionIdentifier {
        public bool Identify (Exception ex, params object?[] entities) {
            if (ex is DbException dbEx) {
                return dbEx.IsTransient;
            }
            return false;
        }
    }
}
