using Application.DtoModels;
using SharedKernel.Exceptions;

namespace Application.FailedResponseHandling.Convertors {
    public class AppExceptionConvertor : IExceptionConvertor {
        public FailedResponseDTO? ToFailedResponse (Exception ex) {
            if (ex is AppException appEx) {
                if (appEx.StatusCode != null) {
                    return new FailedResponseDTO((int)appEx.StatusCode, appEx.Message);
                }
            }

            return null;
        }
    }
}
