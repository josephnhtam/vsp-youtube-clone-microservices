using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;

namespace VideoManager.API.Application.Commands.Handlers {
    public class SetVideoPublishStatusCommandHandler : ICommandHandler<SetVideoPublishStatusCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetVideoPublishStatusCommandHandler> _logger;

        public SetVideoPublishStatusCommandHandler (
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            ILogger<SetVideoPublishStatusCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (SetVideoPublishStatusCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.PublishStatusVersion < request.Version) {
                    video.SetPublishStatus(request.IsPublished, request.Date, request.Version);
                    video.IncrementVersion();
                    await _unitOfWork.CommitAsync();

                    if (request.IsPublished) {
                        _logger.LogInformation("Video ({VideoId}) is published", request.VideoId);
                    } else {
                        _logger.LogInformation("Video ({VideoId}) is unpublished", request.VideoId);
                    }
                }
            });

            return Unit.Value;
        }

    }
}
