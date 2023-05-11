using Application.Handlers;
using Domain.Contracts;
using Library.Domain.Contracts;
using Library.Infrastructure.Contracts;
using MediatR;

namespace Library.API.Application.Commands.Handlers {
    public class DeleteUserProfileCommandHandler : ICommandHandler<DeleteUserProfileCommand> {

        private readonly IUserProfileRepository _repository;
        private readonly ICachedUserProfileRepository _cachedRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUserProfileCommandHandler> _logger;

        public DeleteUserProfileCommandHandler (
            IUserProfileRepository repository,
            ICachedUserProfileRepository cachedRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteUserProfileCommandHandler> logger) {
            _repository = repository;
            _cachedRepository = cachedRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle (DeleteUserProfileCommand request, CancellationToken cancellationToken) {
            await _repository.DeleteUserProfileAsync(request.Id);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("User profile ({UserId}) is deleted", request.Id);

            try {
                await _cachedRepository.RemoveUserProfileCachesAsync(new string[] { request.Id }, cancellationToken);
                _logger.LogInformation("Removed caching for user profile ({UserId})", request.Id);
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to remove caching for user profile ({UserId})", request.Id);
            }

            return Unit.Value;
        }
    }
}
