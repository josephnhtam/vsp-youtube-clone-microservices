using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class CompleteVideoProcessingCommandHandler : ICommandHandler<CompleteVideoProcessingCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompleteVideoProcessingCommandHandler> _logger;

        public CompleteVideoProcessingCommandHandler (IVideoRepository videoRepository, IUnitOfWork unitOfWork, ILogger<CompleteVideoProcessingCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (CompleteVideoProcessingCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.ProcessingStatus >= VideoProcessingStatus.VideoProcessed) {
                    return;
                }

                video.SetThumbnails(request.Thumbnails, request.PreviewThumbnail);
                video.SetVideos(request.Videos);
                video.SetVideoProcssedStatus();
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's processing is complete", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
