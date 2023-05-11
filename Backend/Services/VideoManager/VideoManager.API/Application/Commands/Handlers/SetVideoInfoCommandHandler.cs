using Application.Handlers;
using Domain.Contracts;
using SharedKernel.Exceptions;
using VideoManager.API.Commands;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class SetVideoInfoCommandHandler : ICommandHandler<SetVideoInfoCommand, Video?> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetVideoInfoCommandHandler> _logger;

        public SetVideoInfoCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork, ILogger<SetVideoInfoCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Video> Handle (SetVideoInfoCommand request, CancellationToken cancellationToken) {
            Video? video = null;

            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                video = await _repository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    throw new AppException($"Video {request.VideoId} not found", null, StatusCodes.Status404NotFound);
                }

                if (video.CreatorId != request.CreatorId) {
                    throw new AppException("Unauthorized", null, StatusCodes.Status403Forbidden);
                }

                bool updated = false;

                if (request.SetVideoBasicInfo != null) {
                    video.SetInfo(
                        request.SetVideoBasicInfo.Title,
                        request.SetVideoBasicInfo.Description,
                        request.SetVideoBasicInfo.Tags);

                    video.SetThumbnailId(
                        request.SetVideoBasicInfo.ThumbnailId);

                    updated = true;
                }

                if (request.SetVideoVisibilityInfo != null) {
                    video.SetVisibility(request.SetVideoVisibilityInfo.Visibility);
                    updated = true;
                }

                if (updated) {
                    video.IncrementVersion();
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Video ({VideoId}) info is updated", video.Id);
                }
            });

            return video!;
        }

    }
}
