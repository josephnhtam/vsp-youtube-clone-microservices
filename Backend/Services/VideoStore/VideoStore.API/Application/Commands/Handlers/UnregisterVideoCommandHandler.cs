using Application.Handlers;
using Domain.Contracts;
using MediatR;
using VideoStore.Domain.Contracts;

namespace VideoStore.API.Application.Commands.Handlers {
    public class UnregisterVideoCommandHandler : ICommandHandler<UnregisterVideoCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnregisterVideoCommandHandler> _logger;

        public UnregisterVideoCommandHandler (
            IVideoRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<UnregisterVideoCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UnregisterVideoCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _repository.GetVideoByIdAsync(request.VideoId, cancellationToken);

                if (video != null) {
                    video.SetUnregisteredStatus();
                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogInformation("Video ({VideoId}) is unregistered.", request.VideoId);
                } else {
                    _logger.LogInformation("Video ({VideoId}) not found.", request.VideoId);
                }
            });

            return Unit.Value;
        }

    }
}
