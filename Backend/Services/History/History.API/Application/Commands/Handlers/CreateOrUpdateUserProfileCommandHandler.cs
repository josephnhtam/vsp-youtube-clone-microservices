using Application.Handlers;
using Domain.Contracts;
using History.Domain.Contracts;
using History.Domain.Models;
using History.Infrastructure.Contracts;
using MediatR;
using SharedKernel.Exceptions;

namespace History.API.Application.Commands.Handlers {
    public class CreateOrUpdateUserProfileCommandHandler : ICommandHandler<CreateOrUpdateUserProfileCommand> {

        private readonly IUserProfileRepository _repository;
        private readonly ICachedUserProfileRepository _cachedRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOrUpdateUserProfileCommandHandler> _logger;

        public CreateOrUpdateUserProfileCommandHandler (
            IUserProfileRepository repository,
            ICachedUserProfileRepository cachedRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateOrUpdateUserProfileCommandHandler> logger) {
            _repository = repository;
            _cachedRepository = cachedRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (CreateOrUpdateUserProfileCommand request, CancellationToken cancellationToken) {
            bool create = false;

            if (request.UpdateIfExists) {
                try {
                    await _unitOfWork.ExecuteTransactionAsync(async () => {
                        var userProfile = await _repository.GetUserProfileAsync(request.Id, true, cancellationToken);

                        if (userProfile != null) {
                            userProfile.Update(request.DisplayName, request.Handle, request.ThumbnailUrl, request.Version);
                            await _unitOfWork.CommitAsync(cancellationToken);
                            _logger.LogInformation("User profile ({UserId}) is updated", request.Id);

                            try {
                                await _cachedRepository.RemoveUserProfileCachesAsync(new string[] { request.Id }, cancellationToken);
                                _logger.LogInformation("Removed caching for user profile ({UserId})", request.Id);
                            } catch (Exception ex) {
                                _logger.LogError(ex, "Failed to remove caching for user profile ({UserId})", request.Id);
                            }
                        } else {
                            create = true;
                        }
                    });
                } catch (Exception ex) {
                    _logger.LogError(ex, "An error occurred when updating user profile ({UserId})", request.Id);
                    throw;
                }
            } else {
                create = true;
            }

            if (create) {
                try {
                    var userProfile = UserProfile.Create(request.Id, request.DisplayName, request.Handle, request.ThumbnailUrl, request.Version);
                    await _repository.AddUserProfileAsync(userProfile, cancellationToken);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    _logger.LogInformation("User profile ({UserId}) is created", request.Id);
                } catch (Exception ex) {
                    if (ex.Identify(ExceptionCategories.UniqueViolation)) {
                        _logger.LogWarning("User profile ({UserId}) already exists", request.Id);
                    } else {
                        _logger.LogError(ex, "An error occurred when adding user profile ({UserId})", request.Id);
                        throw;
                    }
                }
            }

            return Unit.Value;
        }
    }
}
