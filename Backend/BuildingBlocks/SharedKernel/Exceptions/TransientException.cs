namespace SharedKernel.Exceptions {
    public class TransientException : Exception {
        public TransientException (string? message = null, Exception? innerException = null) : base(message, innerException) {
        }
    }
}
