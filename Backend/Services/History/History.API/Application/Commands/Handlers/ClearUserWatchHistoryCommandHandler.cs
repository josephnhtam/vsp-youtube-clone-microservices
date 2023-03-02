using Application.Handlers;
using History.Infrastructure.Contracts;
using MediatR;

namespace History.API.Application.Commands.Handlers {
    public class ClearUserWatchHistoryCommandHandler : ICommandHandler<ClearUserWatchHistoryCommand> {

        private readonly IUserHistoryCommandManager _manager;

        public ClearUserWatchHistoryCommandHandler (IUserHistoryCommandManager manager) {
            _manager = manager;
        }

        public async Task<Unit> Handle (ClearUserWatchHistoryCommand request, CancellationToken cancellationToken) {
            await _manager.ClearUserWatchHistory(request.UserId, cancellationToken);
            return Unit.Value;
        }

    }
}
