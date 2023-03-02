using Application.Handlers;
using MediatR;
using Search.Infrastructure.Contracts;

namespace Search.API.Application.Commands.Handlers {
    public class UpdateVideoSearchInfoViewsMetricsCommandHandler : ICommandHandler<UpdateVideoSearchInfoViewsMetricsCommand> {

        private readonly IVideosCommandManager _manager;

        public UpdateVideoSearchInfoViewsMetricsCommandHandler (IVideosCommandManager manager) {
            _manager = manager;
        }

        public async Task<Unit> Handle (UpdateVideoSearchInfoViewsMetricsCommand request, CancellationToken cancellationToken) {
            await _manager.UpdateVideoViewsMetricsAsync(request.VideoId, request.ViewsCount, cancellationToken);
            return Unit.Value;
        }

    }
}
