using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using MediatR;

namespace Library.API.Application.Commands.Handlers {
    public class UpdateVideoViewsMetricsCommandHandler : ICommandHandler<UpdateVideoViewsMetricsCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVideoViewsMetricsCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (UpdateVideoViewsMetricsCommand request, CancellationToken cancellationToken) {
            var video = await _repository.GetVideoByIdAsync(request.VideoId, false, cancellationToken);

            if (video == null) {
                throw new Exception($"Video ({request.VideoId}) not found");
            }

            video.UpdateViewsMetrics(request.ViewsCount, request.UpdateDate);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
