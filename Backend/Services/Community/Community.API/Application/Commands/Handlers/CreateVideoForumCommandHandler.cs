using Application.Handlers;
using Community.Domain.Contracts;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;

namespace Community.API.Application.Commands.Handlers {
    public class CreateVideoForumCommandHandler : ICommandHandler<CreateVideoForumCommand> {

        private readonly IVideoForumRepository _repository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateVideoForumCommandHandler> _logger;

        public CreateVideoForumCommandHandler (
            IVideoForumRepository forumRepository,
            IUserProfileRepository userProfileRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateVideoForumCommandHandler> logger) {
            _repository = forumRepository;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (CreateVideoForumCommand request, CancellationToken cancellationToken) {
            var userProfile = await _userProfileRepository.GetUserProfileAsync(request.CreatorProfile.Id);

            if (userProfile == null) {
                throw new AppException("User not found", null, StatusCodes.Status404NotFound);
            }

            try {
                var videoForum = userProfile.AddVideoForum(request.VideoId, request.AllowedToComment);
                await _repository.AddVideoForumAsync(videoForum);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video forum ({VideoId}) is created", request.VideoId);
            } catch (Exception ex) {
                if (ex.Identify(ExceptionCategories.UniqueViolation)) {
                    _logger.LogWarning("Video forum ({VideoId}) is already created", request.VideoId);
                } else {
                    throw;
                }
            }

            return Unit.Value;
        }
    }
}
