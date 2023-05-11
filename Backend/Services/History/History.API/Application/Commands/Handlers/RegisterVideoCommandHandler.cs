using Application.Handlers;
using Domain.Contracts;
using History.Domain.Contracts;
using History.Domain.Models;
using MediatR;
using SharedKernel.Exceptions;

namespace History.API.Application.Commands.Handlers {
    public class RegisterVideoCommandHandler : ICommandHandler<RegisterVideoCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterVideoCommandHandler> _logger;

        public RegisterVideoCommandHandler (
            IVideoRepository repository,
            IUserProfileRepository userProfileRepository,
            IUnitOfWork unitOfWork,
            ILogger<RegisterVideoCommandHandler> logger) {
            _repository = repository;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (RegisterVideoCommand request, CancellationToken cancellationToken) {
            var userProfile = await _userProfileRepository.GetUserProfileAsync(request.CreatorProfile.Id, false, cancellationToken);

            if (userProfile == null) {
                throw new AppException("User not found", null, StatusCodes.Status404NotFound);
            }

            try {
                var video = Video.Create(
                    request.VideoId,
                    request.CreatorProfile.Id,
                    request.Title,
                    request.Description,
                    request.Tags,
                    request.Visibility,
                    request.CreateDate);

                await _repository.AddVideoAsync(video, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Video ({VideoId}) is registered", request.VideoId);
            } catch (Exception ex) {
                if (ex.Identify(ExceptionCategories.UniqueViolation)) {
                    _logger.LogWarning("Video ({VideoId}) is already registered", request.VideoId);
                } else {
                    throw;
                }
            }

            return Unit.Value;
        }
    }
}
