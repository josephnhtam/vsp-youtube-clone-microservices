using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;

namespace VideoManager.API.Application.Commands.Handlers {
    public class UpdateVideoCommentsMetricsCommandHandler : ICommandHandler<UpdateVideoCommentsMetricsCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompleteVideoProcessingCommandHandler> _logger;

        public UpdateVideoCommentsMetricsCommandHandler (IVideoRepository videoRepository, IUnitOfWork unitOfWork, ILogger<CompleteVideoProcessingCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateVideoCommentsMetricsCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.Metrics.CommentsCountUpdateDate >= request.UpdateDate) {
                    return;
                }

                video.UpdateCommentsMetrics(request.CommentsCount, request.UpdateDate);
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's comments metrics is synchronized", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
