using Application.Handlers;
using Domain.Contracts;
using Domain.TransactionalEvents.Contracts;
using MediatR;
using SharedKernel.Exceptions;
using VideoStore.Domain.Contracts;

namespace VideoStore.API.Application.Commands.Handlers {
    public class RegisterVideoCommandHandler : ICommandHandler<RegisterVideoCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterVideoCommandHandler> _logger;

        public RegisterVideoCommandHandler (
            IVideoRepository repository,
            IUserProfileRepository userProfileRepository,
            ITransactionalEventsContext transactionalEventsContext,
            IUnitOfWork unitOfWork,
            ILogger<RegisterVideoCommandHandler> logger) {
            _repository = repository;
            _userProfileRepository = userProfileRepository;
            _transactionalEventsContext = transactionalEventsContext;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (RegisterVideoCommand request, CancellationToken cancellationToken) {
            var userProfile = await _userProfileRepository.GetUserProfileAsync(request.CreatorProfile.Id, cancellationToken);

            if (userProfile == null) {
                throw new AppException($"User ({request.CreatorProfile.Id}) not found", null, StatusCodes.Status404NotFound);
            }

            if (userProfile == null) {
                throw new TransientException();
            }

            try {
                var video = userProfile.AddVideo(
                    request.VideoId,
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
