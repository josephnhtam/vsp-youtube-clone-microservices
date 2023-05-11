using Application.Handlers;
using Domain.Contracts;
using MediatR;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Commands.Handlers {
    public class FailUserProfileRegistrationCommandHandler : ICommandHandler<FailUserProfileRegistrationCommand> {

        private readonly IUserProfileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FailUserProfileRegistrationCommandHandler> _logger;

        public FailUserProfileRegistrationCommandHandler (
            IUserProfileRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<FailUserProfileRegistrationCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (FailUserProfileRegistrationCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var userProfile = await _repository.GetUserProfileByIdAsync(request.UserId, true, cancellationToken);

                if (userProfile == null) {
                    _logger.LogError("User profile ({UserId}) not found", request.UserId);
                    throw new Exception($"User profile ({request.UserId}) not found");
                }

                if (userProfile.Status >= UserProfileStatus.Registered) {
                    _logger.LogWarning("User profile ({UserId}) registration is already complete or failed", request.UserId);
                    return;
                }

                userProfile.SetRegistrationFailedStatus();

                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("User profile ({UserId}) registration is failed", request.UserId);
            });

            return Unit.Value;
        }

    }
}
