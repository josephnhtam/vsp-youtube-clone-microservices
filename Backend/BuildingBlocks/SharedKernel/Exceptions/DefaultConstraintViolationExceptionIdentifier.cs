namespace SharedKernel.Exceptions {
    public class DefaultConstraintViolationExceptionIdentifier : IExceptionIdentifier {
        public bool Identify (Exception ex, params object?[] entities) {
            return ex is ConstraintViolationException;
        }
    }
}
