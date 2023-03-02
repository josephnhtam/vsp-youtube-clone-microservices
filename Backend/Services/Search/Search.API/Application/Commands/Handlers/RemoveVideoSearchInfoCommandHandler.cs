using Application.Handlers;
using MediatR;
using Search.Infrastructure.Contracts;

namespace Search.API.Application.Commands.Handlers {
    public class RemoveVideoSearchInfoCommandHandler : ICommandHandler<RemoveVideoSearchInfoCommand> {

        private readonly IVideosCommandManager _manager;
        private readonly ILogger<RemoveVideoSearchInfoCommandHandler> _logger;

        public RemoveVideoSearchInfoCommandHandler (IVideosCommandManager manager, ILogger<RemoveVideoSearchInfoCommandHandler> logger) {
            _manager = manager;
            _logger = logger;
        }

        public async Task<Unit> Handle (RemoveVideoSearchInfoCommand request, CancellationToken cancellationToken) {
            await _manager.DeleteVideoAsync(request.VideoId, request.Version, cancellationToken);
            _logger.LogInformation("Video search info ({VideoId}) is deleted", request.VideoId);
            return Unit.Value;
        }

    }
}
