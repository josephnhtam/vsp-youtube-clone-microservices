using MongoDB.Driver;
using SharedKernel.Exceptions;

namespace Infrastructure.MongoDb.Exceptions {
    public class UniqueViolationMongoExceptionIdentifier : IExceptionIdentifier {
        public bool Identify (Exception ex, params object?[] entities) {
            if (ex is MongoWriteException mongoEx) {
                if (mongoEx.WriteError.Category == ServerErrorCategory.DuplicateKey) {
                    return true;
                }
            }

            return false;
        }
    }
}
