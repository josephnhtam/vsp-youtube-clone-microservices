using Application.Handlers;
using Domain.Contracts;
using MediatR;
using Storage.Domain.Contracts;
using Storage.Domain.Models;

namespace Storage.API.Application.Commands.Handlers {
    public class SetFileInUseCommandHandler : ICommandHandler<SetFileInUseCommand> {

        private readonly IFileTrackingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetFileInUseCommandHandler> _logger;

        public SetFileInUseCommandHandler (IFileTrackingRepository repository, IUnitOfWork unitOfWork, ILogger<SetFileInUseCommandHandler> logger) {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (SetFileInUseCommand request, CancellationToken cancellationToken) {
            if (request.FileIds.Count() > 0) {
                await _unitOfWork.ExecuteOptimisticUpdateAsync(async () => {
                    var fileTrackings = await _repository.GetFileTrackingsByFileIdsAsync(request.FileIds, cancellationToken);

                    foreach (var fileTracking in fileTrackings) {
                        if (fileTracking.Status == FileStatus.PendingToRemove) {
                            fileTracking.SetInUse();
                        }
                    }

                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogInformation("File ({FileIds}) currently in use", string.Format(", ", request.FileIds.Select(x => x.ToString())));
                });
            }

            return Unit.Value;
        }

    }
}
