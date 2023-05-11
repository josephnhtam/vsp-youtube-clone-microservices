using Domain.Contracts;
using EventBus;
using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;
using VideoProcessor.Application.Configurations;
using VideoProcessor.Application.IntegrationEvents.VideoManager;
using VideoProcessor.Domain.Contracts;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.IntegrationEventHandlers.VideoManager {
    public class RequestVideoProcessingIntegrationEventHandler : IntegrationEventHandler<
        RequestVideoProcessingIntegrationEvent,
        RequestVideoProcessingIntegrationEventQueue> {

        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly VideoProcessorConfiguration _config;
        private readonly ILogger<RequestVideoProcessingIntegrationEventHandler> _logger;

        public RequestVideoProcessingIntegrationEventHandler (
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            IOptions<VideoProcessorConfiguration> config,
            ILogger<RequestVideoProcessingIntegrationEventHandler> logger) {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _config = config.Value;
            _logger = logger;
        }

        public override async Task Handle (RequestVideoProcessingIntegrationEvent integrationEvent,
            IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            var videoProcessingSteps = _config.VideoProcessingSteps
                .Select(x => VideoProcessingStep.Create(x.Label, x.Height)).ToList();

            var video = Video.Create(
                integrationEvent.VideoId,
                integrationEvent.CreatorId,
                integrationEvent.OriginalFileName,
                integrationEvent.VideoFileUrl,
                videoProcessingSteps);

            try {
                await _videoRepository.AddVideoAsync(video);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation(
                    "Video processing ({VideoId}) is queued.\n" +
                    "Creator Id: {CreatorId}\n" +
                    "Original File Name: {OriginalFileName}\n" +
                    "Video File Url: {VideoFileUrl}", video.Id, video.CreatorId, video.OriginalFileName, video.VideoFileUrl);
            } catch (Exception ex) when (ex.Identify(ExceptionCategories.UniqueViolation)) {
                _logger.LogInformation("Video processing ({VideoId}) is already queued.", video.Id);
            }
        }

    }

    public class RequestVideoProcessingIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "VideoProcessor." + properties.QueueName;
        }
    }
}
