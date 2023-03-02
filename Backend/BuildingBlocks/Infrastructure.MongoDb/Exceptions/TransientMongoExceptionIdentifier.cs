using MongoDB.Driver;
using SharedKernel.Exceptions;

namespace Infrastructure.MongoDb.Exceptions {
    public class TransientMongoExceptionIdentifier : IExceptionIdentifier {
        public bool Identify (Exception ex, params object?[] entities) {
            if (ex is MongoException mongoEx) {
                if (mongoEx.HasErrorLabel("TransientTransactionError") ||
                    mongoEx.HasErrorLabel("UnknownTransactionCommitResult")) {
                    return true;
                }

                if (ex is MongoWriteException wEx) {
                    if (wEx.WriteError.Category != ServerErrorCategory.DuplicateKey) {
                        return true;
                    }
                }

                if (ex is MongoConnectionException ||
                    ex is MongoExecutionTimeoutException ||
                    ex is MongoNotPrimaryException ||
                    ex is MongoNodeIsRecoveringException ||
                    ex is MongoCursorNotFoundException) {
                    return true;
                }
            }

            return false;
        }
    }
}
