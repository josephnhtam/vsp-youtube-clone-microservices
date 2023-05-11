using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class SetVideoRegisteredStatusCommandHandler : ICommandHandler<SetVideoRegisteredStatusCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetVideoRegisteredStatusCommandHandler> _logger;

        public SetVideoRegisteredStatusCommandHandler (
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            ILogger<SetVideoRegisteredStatusCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (SetVideoRegisteredStatusCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.Status >= VideoStatus.Registered) {
                    _logger.LogWarning("Video ({VideoId}) is already registered", request.VideoId);
                    return;
                }

                video.SetVideoRegistered();
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) is registered", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
