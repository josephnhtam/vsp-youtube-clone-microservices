using Application.Handlers;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;
using Users.API.Application.Services;
using Users.API.Application.Utilities;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Commands.Handlers {
    public class CreateUserProfileCommandHandler : ICommandHandler<CreateUserProfileCommand> {

        private readonly IUserProfileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageValidator _imageValidator;
        private readonly ILogger<CreateUserProfileCommandHandler> _logger;

        public CreateUserProfileCommandHandler (
            IUserProfileRepository repository,
            IUnitOfWork unitOfWork,
            IImageValidator imageValidator,
            ILogger<CreateUserProfileCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _imageValidator = imageValidator;
            _logger = logger;
        }

        public async Task<Unit> Handle (CreateUserProfileCommand request, CancellationToken cancellationToken) {
            try {
                var thumbnailFile = !string.IsNullOrEmpty(request.ThumbnailToken) ?
                    _imageValidator.ValidateImageToken(
                        request.ThumbnailToken, Categories.UserUploadedThumbnail, request.UserId) :
                    null;

                var userProfile = UserProfile.Create(request.UserId, request.DisplayName, thumbnailFile);

                await _repository.AddUserProfileAsync(userProfile);
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("User profile ({UserId}) is created", request.UserId);
            } catch (Exception ex) {
                if (ex.Identify(ExceptionCategories.UniqueViolation)) {
                    _logger.LogWarning(ex, "User profile ({UserId}) has already been created", request.UserId);
                    throw new AppException($"User profile ({request.UserId}) has already been created", null, StatusCodes.Status409Conflict);
                }

                _logger.LogError(ex, "An error occurred when creating user profile ({UserId})", request.UserId);
                throw;
            }

            return Unit.Value;
        }
    }
}
