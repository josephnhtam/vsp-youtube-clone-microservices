namespace SharedKernel.Exceptions {
    public class UniqueViolationException : Exception {
        public UniqueViolationException (string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
