using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoStore.Domain.Contracts;

namespace VideoStore.API.Application.Commands.Handlers {
    public class UpdateVideoResourcesCommandHandler : ICommandHandler<UpdateVideoResourcesCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateVideoResourcesCommandHandler> _logger;

        public UpdateVideoResourcesCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork, ILogger<UpdateVideoResourcesCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateVideoResourcesCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _repository.GetVideoByIdAsync(request.VideoId, cancellationToken);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.Status != Domain.Models.VideoStatus.Preparing) {
                    _logger.LogInformation("The video is already completely prepared");
                    return;
                }

                var videos = request.Merge ? video.Videos.Union(request.Videos).ToList() : request.Videos;

                video.SetVideoResources(videos);
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's resources updated", video.Id);
            });

            return Unit.Value;
        }

    }
}
