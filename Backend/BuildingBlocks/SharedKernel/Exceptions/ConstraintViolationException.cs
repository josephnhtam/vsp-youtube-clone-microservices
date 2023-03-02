namespace SharedKernel.Exceptions {
    public class ConstraintViolationException : Exception {
        public ConstraintViolationException (string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
