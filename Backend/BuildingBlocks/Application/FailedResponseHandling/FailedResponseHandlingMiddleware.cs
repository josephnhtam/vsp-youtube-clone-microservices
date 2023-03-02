using Application.DtoModels;
using Application.FailedResponseHandling.Convertors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Application.FailedResponseHandling {
    public class FailedResponseFilter : IAsyncActionFilter {

        private readonly ILogger<FailedResponseFilter> _logger;

        public FailedResponseFilter (ILogger<FailedResponseFilter> logger) {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync (ActionExecutingContext context, ActionExecutionDelegate next) {
            var executedContext = await next.Invoke();

            if (!executedContext.ExceptionHandled && executedContext.Exception != null) {
                var failedResponse = executedContext.Exception.ToFailedResponse();

                executedContext.ExceptionHandled = true;
                executedContext.Result = new ObjectResult(failedResponse) {
                    StatusCode = failedResponse.StatusCode,
                };

                if (failedResponse.StatusCode == StatusCodes.Status500InternalServerError) {
                    _logger.LogError(executedContext.Exception, "An internal server error occurred when responding to user");
                }
            } else {
                if (executedContext.Result is ObjectResult objectResult) {
                    if (objectResult.StatusCode >= 400 && objectResult.StatusCode < 500) {
                        if (objectResult.Value is string msg) {
                            var failedResponse = new FailedResponseDTO(objectResult.StatusCode.Value, msg);

                            executedContext.Result = new ObjectResult(failedResponse) {
                                StatusCode = failedResponse.StatusCode,
                            };
                        }
                    }
                } else if (executedContext.Result is StatusCodeResult statusCodeResult) {
                    if (statusCodeResult.StatusCode >= 400 && statusCodeResult.StatusCode < 500) {
                        var failedResponse = new FailedResponseDTO(statusCodeResult.StatusCode, string.Empty);

                        executedContext.Result = new ObjectResult(failedResponse) {
                            StatusCode = failedResponse.StatusCode,
                        };
                    }
                }
            }
        }


    }
}
