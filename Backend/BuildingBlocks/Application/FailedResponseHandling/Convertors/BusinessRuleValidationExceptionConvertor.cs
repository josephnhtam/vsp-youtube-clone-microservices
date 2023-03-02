using Application.DtoModels;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Application.FailedResponseHandling.Convertors {
    public class BusinessRuleValidationExceptionConvertor : IExceptionConvertor {
        public FailedResponseDTO? ToFailedResponse (Exception ex) {
            if (ex is BusinessRuleValidationException ruleEx) {
                return new FailedResponseDTO(StatusCodes.Status400BadRequest, ruleEx.Message);
            }

            return null;
        }
    }
}
