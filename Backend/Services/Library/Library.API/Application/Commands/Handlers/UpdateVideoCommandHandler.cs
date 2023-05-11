using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using MediatR;

namespace Library.API.Application.Commands.Handlers {
    public class UpdateVideoCommandHandler : ICommandHandler<UpdateVideoCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateVideoCommandHandler> _logger;

        public UpdateVideoCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork, ILogger<UpdateVideoCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateVideoCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var video = await _repository.GetVideoByIdAsync(request.VideoId, true, cancellationToken);

                if (video == null) {
                    throw new Exception($"Video ({request.VideoId}) not found");
                }

                video.Update(
                    request.Title,
                    request.Description,
                    request.Tags,
                    request.ThumbnailUrl,
                    request.PreviewThumbnailUrl,
                    request.LengthSeconds,
                    request.Visibility,
                    request.Status,
                    request.StatusUpdateDate,
                    request.Version);

                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Video ({VideoId}) is updated.", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
