using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class AddProcessedVideoCommandHandler : ICommandHandler<AddProcessedVideoCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddProcessedVideoCommandHandler> _logger;

        public AddProcessedVideoCommandHandler (IVideoRepository videoRepository, IUnitOfWork unitOfWork, ILogger<AddProcessedVideoCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (AddProcessedVideoCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.ProcessingStatus >= VideoProcessingStatus.VideoProcessed ||
                    video.Videos.Any(x => x.VideoFileId == request.Video.VideoFileId)) {
                    return;
                }

                video.AddProcessedVideo(request.Video);
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's processed video ({VideoFileId}) is added", request.VideoId, request.Video.VideoFileId);
            });

            return Unit.Value;
        }

    }
}
