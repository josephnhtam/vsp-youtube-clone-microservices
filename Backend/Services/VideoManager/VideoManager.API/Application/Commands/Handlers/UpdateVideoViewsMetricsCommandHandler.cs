using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoManager.Domain.Contracts;

namespace VideoManager.API.Application.Commands.Handlers {
    public class UpdateVideoViewsMetricsCommandHandler : ICommandHandler<UpdateVideoViewsMetricsCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompleteVideoProcessingCommandHandler> _logger;

        public UpdateVideoViewsMetricsCommandHandler (IVideoRepository videoRepository, IUnitOfWork unitOfWork, ILogger<CompleteVideoProcessingCommandHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateVideoViewsMetricsCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.Metrics.ViewsCountUpdateDate >= request.UpdateDate) {
                    return;
                }

                video.UpdateViewsMetrics(request.ViewsCount, request.UpdateDate);
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) 's Views metrics is synchronized", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
