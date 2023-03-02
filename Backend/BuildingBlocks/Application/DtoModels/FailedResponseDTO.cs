namespace Application.DtoModels {
    public class FailedResponseDTO {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public IDictionary<string, string>? Errors { get; set; }

        public FailedResponseDTO (int statusCode, string message) {
            StatusCode = statusCode;
            Message = message;
        }

        public FailedResponseDTO (int statusCode, string message, IDictionary<string, string>? errors) : this(statusCode, message) {
            Errors = errors;
        }
    }
}
