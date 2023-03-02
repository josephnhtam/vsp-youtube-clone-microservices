using Application.Handlers;
using MediatR;
using Search.Infrastructure.Contracts;

namespace Search.API.Application.Commands.Handlers {
    public class UpdateVideoSearchInfoVotesMetricsCommandHandler : ICommandHandler<UpdateVideoSearchInfoVotesMetricsCommand> {

        private readonly IVideosCommandManager _manager;

        public UpdateVideoSearchInfoVotesMetricsCommandHandler (IVideosCommandManager manager) {
            _manager = manager;
        }

        public async Task<Unit> Handle (UpdateVideoSearchInfoVotesMetricsCommand request, CancellationToken cancellationToken) {
            await _manager.UpdateVideoVotesMetricsAsync(request.VideoId, request.LikesCount, request.DislikesCount, cancellationToken);
            return Unit.Value;
        }

    }
}
