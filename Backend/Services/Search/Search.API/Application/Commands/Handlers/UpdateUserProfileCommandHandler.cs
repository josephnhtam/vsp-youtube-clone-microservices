using Application.Handlers;
using MediatR;
using Search.Domain.Models;
using Search.Infrastructure.Contracts;

namespace Search.API.Application.Commands.Handlers {
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand> {

        private readonly IVideosCommandManager _manager;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

        public UpdateUserProfileCommandHandler (IVideosCommandManager manager, ILogger<UpdateUserProfileCommandHandler> logger) {
            _manager = manager;
            _logger = logger;
        }

        public async Task<Unit> Handle (UpdateUserProfileCommand request, CancellationToken cancellationToken) {
            var userProfile = new UserProfile {
                Id = request.Id,
                DisplayName = request.DisplayName,
                Handle = request.Handle,
                ThumbnailUrl = request.ThumbnailUrl,
                PrimaryVersion = request.Version
            };

            await _manager.UpdateUserProfileAsync(userProfile, cancellationToken);
            _logger.LogInformation("User profile ({UserId}) is updated", request.Id);
            return Unit.Value;
        }

    }
}
