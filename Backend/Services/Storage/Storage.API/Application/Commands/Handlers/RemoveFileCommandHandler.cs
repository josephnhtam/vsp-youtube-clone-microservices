using Application.Handlers;
using Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Options;
using Storage.API.Application.Configurations;
using Storage.Domain.Contracts;
using Storage.Domain.Models;

namespace Storage.API.Application.Commands.Handlers {
    public class RemoveFileCommandHandler : ICommandHandler<RemoveFileCommand> {

        private readonly IFileTrackingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CleanupConfiguration _config;
        private readonly ILogger<RemoveFileCommandHandler> _logger;

        public RemoveFileCommandHandler (IFileTrackingRepository repository, IUnitOfWork unitOfWork, IOptions<CleanupConfiguration> config, ILogger<RemoveFileCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<Unit> Handle (RemoveFileCommand request, CancellationToken cancellationToken) {
            if (request.FileIds.Count() > 0) {
                TimeSpan removalDelay = request.RemovalDelay ?? TimeSpan.FromHours(_config.DefaultFileRemovalDelayHours);

                await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                    var fileTrackings = await _repository.GetFileTrackingsByFileIdsAsync(request.FileIds, cancellationToken);

                    foreach (var fileTracking in fileTrackings) {
                        if (fileTracking.Status == FileStatus.InUse) {
                            fileTracking.SetPendingToRemove(removalDelay);
                        }
                    }

                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogInformation("File removal ({FileIds}) is pended", string.Join(", ", request.FileIds.Select(x => x.ToString())));
                });
            }

            return Unit.Value;
        }

    }
}
