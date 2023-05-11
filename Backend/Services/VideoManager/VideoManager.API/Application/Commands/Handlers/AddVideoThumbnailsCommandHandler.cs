using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class AddVideoThumbnailsCommandHandler : ICommandHandler<AddVideoThumbnailsCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddVideoThumbnailsCommandHandler> _logger;

        public AddVideoThumbnailsCommandHandler (IVideoRepository videoRepository, IUnitOfWork unitOfWork, ILogger<AddVideoThumbnailsCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (AddVideoThumbnailsCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.ThumbnailStatus == VideoThumbnailStatus.Processed) {
                    return;
                }

                video.SetThumbnails(request.Thumbnails, request.PreviewThumbnail);
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's thumbnails are added", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
