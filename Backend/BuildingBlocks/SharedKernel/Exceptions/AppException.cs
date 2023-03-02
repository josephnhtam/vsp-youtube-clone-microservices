namespace SharedKernel.Exceptions {
    public class AppException : Exception {
        public int? StatusCode { get; set; }

        public AppException (string? message, Exception? inner, int? statusCode)
            : base(message, inner) {
            StatusCode = statusCode;
        }
    }
}
