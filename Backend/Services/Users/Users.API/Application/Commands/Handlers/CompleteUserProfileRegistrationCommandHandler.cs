using Application.Handlers;
using Domain.Contracts;
using MediatR;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Commands.Handlers {
    public class CompleteUserProfileRegistrationCommandHandler : ICommandHandler<CompleteUserProfileRegistrationCommand> {

        private readonly IUserProfileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompleteUserProfileRegistrationCommandHandler> _logger;

        public CompleteUserProfileRegistrationCommandHandler (
            IUserProfileRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<CompleteUserProfileRegistrationCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (CompleteUserProfileRegistrationCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var userProfile = await _repository.GetUserProfileByIdAsync(request.UserId, true, cancellationToken);

                if (userProfile == null) {
                    _logger.LogError("User profile ({UserId}) not found", request.UserId);
                    throw new Exception($"User profile ({request.UserId}) not found");
                }

                if (userProfile.Status >= UserProfileStatus.Registered) {
                    _logger.LogInformation("User profile ({UserId}) registration is already complete or failed", request.UserId);
                    return;
                }

                userProfile.SetRegisteredStatus();

                await _unitOfWork.CommitAsync(cancellationToken);
            });

            return Unit.Value;
        }

    }
}
