using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoStore.Domain.Contracts;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.Commands.Handlers {
    public class SetVideoReadyCommandHandler : ICommandHandler<SetVideoReadyCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateVideoResourcesCommandHandler> _logger;

        public SetVideoReadyCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork, ILogger<UpdateVideoResourcesCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (SetVideoReadyCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _repository.GetVideoByIdAsync(request.VideoId, cancellationToken);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.Status == VideoStatus.Preparing) {
                    _logger.LogInformation("Updating video ({VideoId}) to be in ready state", video.Id);

                    video.SetInfo(
                        request.Title,
                        request.Description,
                        request.Tags,
                        request.ThumbnailUrl,
                        request.PreviewThumbnailUrl,
                        request.Visibility,
                        request.AllowedToPublish,
                        request.InfoVersion,
                        true);
                    video.SetVideoResources(request.Videos);
                    video.SetReadyStatus();
                    video.IncrementVersion();

                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Video ({VideoId}) is now in {VideoStatus} state", video.Id, video.Status);
                } else {
                    _logger.LogWarning("Video ({VideoId}) is not in preparing state", video.Id);
                }
            });

            return Unit.Value;
        }

    }
}
