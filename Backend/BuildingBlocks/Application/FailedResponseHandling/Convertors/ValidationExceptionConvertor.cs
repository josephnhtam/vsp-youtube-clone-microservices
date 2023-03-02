using Application.DtoModels;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.FailedResponseHandling.Convertors {
    public class ValidationExceptionConvertor : IExceptionConvertor {
        public FailedResponseDTO? ToFailedResponse (Exception ex) {
            if (ex is ValidationException valEx) {
                var errors = valEx.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(x => x.Key, x => x.First().ErrorMessage);

                if (errors.Count == 1) {
                    return new FailedResponseDTO(StatusCodes.Status400BadRequest, errors.First().Value, errors);
                } else {
                    return new FailedResponseDTO(StatusCodes.Status400BadRequest, "Invalid request", errors);
                }
            }

            return null;
        }
    }
}
