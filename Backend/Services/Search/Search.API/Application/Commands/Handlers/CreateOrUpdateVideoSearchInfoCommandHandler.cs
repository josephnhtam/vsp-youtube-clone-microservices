using Application.Handlers;
using AutoMapper;
using MediatR;
using Search.Domain.Models;
using Search.Infrastructure.Contracts;

namespace Search.API.Application.Commands.Handlers {
    public class CreateOrUpdateVideoSearchInfoCommandHandler : ICommandHandler<CreateOrUpdateVideoSearchInfoCommand> {

        private readonly IVideosCommandManager _manager;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOrUpdateVideoSearchInfoCommandHandler> _logger;

        public CreateOrUpdateVideoSearchInfoCommandHandler (IVideosCommandManager manager, IMapper mapper, ILogger<CreateOrUpdateVideoSearchInfoCommandHandler> logger) {
            _manager = manager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle (CreateOrUpdateVideoSearchInfoCommand request, CancellationToken cancellationToken) {
            var video = CreateVideoFromRequest(request);
            await _manager.IndexVideoAsync(video, request.Version, cancellationToken);
            _logger.LogInformation("Video search info ({VideoId}) is created or updated", request.VideoId);
            return Unit.Value;
        }

        private Video CreateVideoFromRequest (CreateOrUpdateVideoSearchInfoCommand request) {
            var video = new Video {
                Id = request.VideoId.ToString(),
                CreatorProfile = _mapper.Map<UserProfile>(request.CreatorProfile),
                Title = request.Title,
                Description = request.Description,
                Contents = null,
                Tags = request.Tags.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToArray(),
                LengthSeconds = request.LengthSeconds,
                Version = request.Version,
                IsDeleted = false,
                ThumbnailUrl = request.ThumbnailUrl,
                PreviewThumbnailUrl = request.PreviewThumbnailUrl,
                Metrics = _mapper.Map<VideoMetrics>(request.Metrics),
                CreateDate = request.CreateDate,
            };
            return video;
        }

    }
}
