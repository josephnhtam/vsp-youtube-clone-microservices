using Application.Handlers;
using Community.Domain.Contracts;
using Domain.Contracts;
using MediatR;

namespace Community.API.Application.Commands.Handlers {
    public class UnregisterVideoForumCommandHandler : ICommandHandler<UnregisterVideoForumCommand> {

        private readonly IVideoForumRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnregisterVideoForumCommandHandler> _logger;

        public UnregisterVideoForumCommandHandler (
            IVideoForumRepository forumRepository,
            IUnitOfWork unitOfWork,
            ILogger<UnregisterVideoForumCommandHandler> logger) {
            _repository = forumRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UnregisterVideoForumCommand request, CancellationToken cancellationToken) {
            var videoForum = await _repository.GetVideoForumByIdAsync(request.VideoId);

            if (videoForum != null) {
                videoForum.SetUnregistered();
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Video forum ({VideoId}) is unregistered.", request.VideoId);
            } else {
                _logger.LogWarning("Video forum ({VideoId}) not found.", request.VideoId);
            }

            return Unit.Value;
        }
    }
}
