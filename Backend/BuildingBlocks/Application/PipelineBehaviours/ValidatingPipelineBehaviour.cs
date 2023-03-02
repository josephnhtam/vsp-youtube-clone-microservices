using Application.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Utilities;

namespace Application.PipelineBehaviours {
    public class ValidatingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IAppRequest<TResponse> {

        private readonly ILogger<ValidatingPipelineBehaviour<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        private static readonly string _requestName;

        static ValidatingPipelineBehaviour () {
            _requestName = typeof(TRequest).GetGenericTypeName();
        }

        public ValidatingPipelineBehaviour (IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatingPipelineBehaviour<TRequest, TResponse>> logger) {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle (TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            _logger.LogDebug("Validating {RequestName}", _requestName);

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any()) {
                var errors = string.Join(", ", failures.Select(x => x.ErrorMessage).Where(x => !string.IsNullOrEmpty(x)));

                if (!string.IsNullOrEmpty(errors)) {
                    _logger.LogWarning("Failed to validate {RequestName} - Errors: {Errors}", _requestName, errors);
                } else {
                    _logger.LogWarning("Failed to validate {RequestName}", _requestName);
                }

                throw new ValidationException("Validation exception", failures);
            }

            return await next();
        }

    }
}
