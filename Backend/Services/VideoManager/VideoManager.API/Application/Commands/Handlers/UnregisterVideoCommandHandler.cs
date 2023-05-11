using Application.Handlers;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class UnregisterVideoCommandHandler : ICommandHandler<UnregisterVideoCommand> {

        private readonly IVideoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnregisterVideoCommandHandler> _logger;

        public UnregisterVideoCommandHandler (IVideoRepository repository, IUnitOfWork unitOfWork, ILogger<UnregisterVideoCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (UnregisterVideoCommand request, CancellationToken cancellationToken) {
            Video? video = null;

            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                video = await _repository.GetVideoByIdAsync(request.VideoId);

                if (video == null) {
                    throw new AppException($"Video {request.VideoId} not found", null, StatusCodes.Status404NotFound);
                }

                if (video.CreatorId != request.UserId) {
                    throw new AppException("Unauthorized", null, StatusCodes.Status403Forbidden);
                }

                if (video.Status == VideoStatus.Unregistered) {
                    return;
                }

                if (video.Status != VideoStatus.RegistrationFailed) {
                    if (video.ProcessingStatus > VideoProcessingStatus.WaitingForUserUpload &&
                        video.ProcessingStatus < VideoProcessingStatus.VideoProcessed) {
                        throw new AppException(null, null, StatusCodes.Status400BadRequest);
                    }
                }

                video.SetUnregistered();
                video.IncrementVersion();

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Video ({VideoId}) is unregistered", request.VideoId);
            });

            return Unit.Value;
        }

    }
}
