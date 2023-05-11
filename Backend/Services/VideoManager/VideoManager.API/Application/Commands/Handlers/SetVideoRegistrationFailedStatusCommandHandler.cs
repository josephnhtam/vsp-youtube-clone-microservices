using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class SetVideoRegistrationFailedStatusCommandHandler : ICommandHandler<SetVideoRegistrationFailedStatusCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetVideoProcessingFailedStatusCommandHandler> _logger;

        public SetVideoRegistrationFailedStatusCommandHandler (
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            ILogger<SetVideoProcessingFailedStatusCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (SetVideoRegistrationFailedStatusCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.Status == VideoStatus.RegistrationFailed) {
                    return;
                }

                video.SetVideoRegistrationFailed();
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's registration failed", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
