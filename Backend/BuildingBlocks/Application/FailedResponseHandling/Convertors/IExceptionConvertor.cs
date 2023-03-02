using Application.DtoModels;

namespace Application.FailedResponseHandling.Convertors {
    public interface IExceptionConvertor {
        FailedResponseDTO? ToFailedResponse (Exception ex);
    }
}
