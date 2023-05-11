using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoStore.Domain.Contracts;

namespace VideoStore.API.Application.Commands.Handlers {
    public class UpdateVideoViewsMetricsCommandHandler : ICommandHandler<UpdateVideoViewsMetricsCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateVideoInfoCommandHandler> _logger;

        public UpdateVideoViewsMetricsCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork, ILogger<UpdateVideoInfoCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateVideoViewsMetricsCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _repository.GetVideoByIdAsync(request.VideoId, cancellationToken);

                if (video == null) {
                    _logger.LogError("Video ({VideoId}) not found", request.VideoId);
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                if (video.Metrics.ViewsCountUpdateDate == null || request.UpdateDate > video.Metrics.ViewsCountUpdateDate) {
                    video.UpdateViewsMetrics(request.ViewsCount, request.UpdateDate);
                    video.IncrementVersion();

                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("Video ({VideoId}) 's views metrics updated", video.Id);
                }
            });

            return Unit.Value;
        }

    }
}
