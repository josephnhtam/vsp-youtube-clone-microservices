using Application.DtoModels;

namespace Application.FailedResponseHandling.Convertors {
    public class HttpRequestExceptionConvertor : IExceptionConvertor {
        public FailedResponseDTO? ToFailedResponse (Exception ex) {
            if (ex is HttpRequestException appEx) {
                if (appEx.StatusCode != null) {
                    return new FailedResponseDTO((int)appEx.StatusCode, appEx.Message);
                }
            }

            return null;
        }
    }
}
