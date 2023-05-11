using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class SetVideoProcessingFailedStatusCommandHandler : ICommandHandler<SetVideoProcessingFailedStatusCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetVideoProcessingFailedStatusCommandHandler> _logger;

        public SetVideoProcessingFailedStatusCommandHandler (
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            ILogger<SetVideoProcessingFailedStatusCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (SetVideoProcessingFailedStatusCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.ProcessingStatus == VideoProcessingStatus.VideoProcessingFailed) {
                    return;
                }

                video.SetVideoProcessingFailedStatus();
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's processing failed", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
