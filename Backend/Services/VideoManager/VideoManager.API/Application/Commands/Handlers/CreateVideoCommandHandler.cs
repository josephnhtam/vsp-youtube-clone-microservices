using Application.Handlers;
using Domain.Contracts;
using VideoManager.API.Commands;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class CreateVideoCommandHandler : ICommandHandler<CreateVideoCommand, Video> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateVideoCommandHandler> _logger;

        public CreateVideoCommandHandler (
            IUserProfileRepository userProfileRepository,
            IVideoRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<CreateVideoCommandHandler> logger) {
            _videoRepository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<Video> Handle (CreateVideoCommand request, CancellationToken cancellationToken) {
            var video = Video.Create(request.CreatorId, request.Title, request.Description);

            await _videoRepository.AddVideoAsync(video);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Video ({VideoId}) is created", video.Id);

            return video;
        }

    }
}
