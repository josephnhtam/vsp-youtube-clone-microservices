using System.Net;

namespace SharedKernel.Exceptions {
    public class TransientHttpExceptionIdentifier : IExceptionIdentifier {
        public bool Identify (Exception ex, params object?[] entities) {
            if (ex is HttpRequestException httpReqEx) {
                if (httpReqEx.StatusCode != null &&
                    (httpReqEx.StatusCode == HttpStatusCode.RequestTimeout ||
                     httpReqEx.StatusCode >= HttpStatusCode.InternalServerError)) {
                    return true;
                }
            }

            return false;
        }
    }
}
