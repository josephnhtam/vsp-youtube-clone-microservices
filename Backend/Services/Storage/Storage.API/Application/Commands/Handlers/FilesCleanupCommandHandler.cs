using Application.Handlers;
using Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Options;
using Storage.API.Application.Configurations;
using Storage.Domain.Contracts;
using Storage.Domain.Models;

namespace Storage.API.Application.Commands.Handlers {
    public class FilesCleanupCommandHandler : ICommandHandler<FilesCleanupCommand> {

        private readonly IFileTrackingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CleanupConfiguration _config;
        private readonly ILogger<FilesCleanupCommandHandler> _logger;

        public FilesCleanupCommandHandler (IFileTrackingRepository repository, IUnitOfWork unitOfWork, IOptions<CleanupConfiguration> config, ILogger<FilesCleanupCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<Unit> Handle (FilesCleanupCommand request, CancellationToken cancellationToken) {
            TimeSpan removalDelay = request.CleanupDelay ?? TimeSpan.FromHours(_config.DefaultFileRemovalDelayHours);

            await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                var fileTrackings = await _repository.GetFileTrackingsByGroupIdAsync(request.GroupId, cancellationToken);

                if (request.ExcludedFileIds != null) {
                    fileTrackings = fileTrackings.Where(x => !request.ExcludedFileIds.Contains(x.FileId));
                }

                if (request.ExcludedCategories != null) {
                    fileTrackings = fileTrackings.Where(x => !request.ExcludedCategories.Contains(x.Category));
                }

                if (request.ExcludedContentTypes != null) {
                    fileTrackings = fileTrackings.Where(x => x.ContentType != null && !request.ExcludedContentTypes.Contains(x.ContentType));
                }

                foreach (var fileTracking in fileTrackings) {
                    if (fileTracking.Status == FileStatus.InUse) {
                        fileTracking.SetPendingToRemove(removalDelay);
                    }
                }

                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("File cleanup ({GroupId}) is pended.\nFile ({FileIds}) will be removed.",
                    request.GroupId, string.Join(", ", fileTrackings.Select(x => x.FileId.ToString())));
            });

            return Unit.Value;
        }

    }
}
