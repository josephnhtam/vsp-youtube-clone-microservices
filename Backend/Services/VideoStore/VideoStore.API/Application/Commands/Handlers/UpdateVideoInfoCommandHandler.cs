using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoStore.Domain.Contracts;

namespace VideoStore.API.Application.Commands.Handlers {
    public class UpdateVideoInfoCommandHandler : ICommandHandler<UpdateVideoInfoCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateVideoInfoCommandHandler> _logger;

        public UpdateVideoInfoCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork, ILogger<UpdateVideoInfoCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateVideoInfoCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _repository.GetVideoByIdAsync(request.VideoId, cancellationToken);

                if (video == null) {
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.InfoVersion < request.InfoVersion) {
                    video.SetInfo(
                        request.Titile,
                        request.Description,
                        request.Tags,
                        request.ThumbnailUrl,
                        request.PreviewThumbnailUrl,
                        request.Visibility,
                        request.AllowedToPublish,
                        request.InfoVersion);

                    video.IncrementVersion();

                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Video ({VideoId}) info is updated", video.Id);
                    _logger.LogInformation("Video ({VideoId}) is currently in {video.Status} state", video.Id);
                }
            });

            return Unit.Value;
        }

    }
}
