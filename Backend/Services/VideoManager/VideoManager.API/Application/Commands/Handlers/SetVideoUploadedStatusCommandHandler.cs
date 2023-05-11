using Application.Handlers;
using Domain.Contracts;
using Domain.TransactionalEvents.Contracts;
using Domain.TransactionalEvents.Outbox;
using MediatR;
using Storage.Shared.IntegrationEvents;
using VideoManager.Domain.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.Commands.Handlers {
    public class SetVideoUploadedStatusCommandHandler : ICommandHandler<SetVideoUploadedStatusCommand> {

        private readonly IVideoRepository _videoRepository;
        private readonly ITransactionalEventsContext _transactionalEventsContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetVideoUploadedStatusCommandHandler> _logger;

        public SetVideoUploadedStatusCommandHandler (
            IVideoRepository videoRepository,
            ITransactionalEventsContext transactionalEventsContext,
            IUnitOfWork unitOfWork,
            ILogger<SetVideoUploadedStatusCommandHandler> logger) {
            _videoRepository = videoRepository;
            _transactionalEventsContext = transactionalEventsContext;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (SetVideoUploadedStatusCommand request, CancellationToken cancellationToken) {
            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var video = await _videoRepository.GetVideoByIdAsync(request.VideoId);

                if (video == null || video.Status == VideoStatus.Unregistered) {
                    _transactionalEventsContext.AddOutboxMessage(new FilesCleanupIntegrationEvent(request.VideoId));
                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogWarning("Video ({VideoId}) not found or unregistered", request.VideoId);
                } else {
                    if (video.ProcessingStatus >= VideoProcessingStatus.VideoUploaded) {
                        return;
                    }

                    video.SetVideoUploadedStatus(request.OriginalFileName, request.Url);
                    video.IncrementVersion();

                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogInformation("Video ({VideoId}) is uploaded", request.VideoId);
                }
            });

            return Unit.Value;
        }

    }
}
