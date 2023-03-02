namespace SharedKernel.Exceptions {
    public class DefaultUniqueViolationExceptionIdentifier : IExceptionIdentifier {
        public bool Identify (Exception ex, params object?[] entities) {
            return ex is UniqueViolationException;
        }
    }
}
