using Application.Handlers;
using History.Infrastructure.Contracts;
using MediatR;

namespace History.API.Application.Commands.Handlers {
    public class RemoveVideoFromUserWatchHistoryCommandHandler : ICommandHandler<RemoveVideoFromUserWatchHistoryCommand> {

        private readonly IUserHistoryCommandManager _manager;

        public RemoveVideoFromUserWatchHistoryCommandHandler (IUserHistoryCommandManager manager) {
            _manager = manager;
        }

        public async Task<Unit> Handle (RemoveVideoFromUserWatchHistoryCommand request, CancellationToken cancellationToken) {
            await _manager.RemoveVideoFromUserWatchHistory(request.UserId, request.VideoId, cancellationToken);
            return Unit.Value;
        }

    }
}
