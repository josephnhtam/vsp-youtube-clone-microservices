using Application.Handlers;
using MediatR;
using Microsoft.Extensions.Options;
using SharedKernel.Exceptions;
using Subscriptions.API.Application.Configurations;
using Subscriptions.Domain.Contracts;
using Subscriptions.Domain.Models;
using Subscriptions.Domain.Specifications;

namespace Subscriptions.API.Application.Commands.Handlers {
    public class PublishVideoUploadedCommandHandler : ICommandHandler<PublishVideoUploadedCommand> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationDataAccess _notificationDataAccess;
        private readonly NotificationConfiguration _config;
        private readonly ILogger<PublishVideoUploadedCommandHandler> _logger;

        public PublishVideoUploadedCommandHandler (
            IUserProfileRepository userProfileRepository,
            ISubscriptionRepository subscriptionRepository,
            INotificationDataAccess notificationDataAccess,
            IOptions<NotificationConfiguration> config,
            ILogger<PublishVideoUploadedCommandHandler> logger) {
            _userProfileRepository = userProfileRepository;
            _subscriptionRepository = subscriptionRepository;
            _notificationDataAccess = notificationDataAccess;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<Unit> Handle (PublishVideoUploadedCommand request, CancellationToken cancellationToken) {
            var senderProfile = await _userProfileRepository.GetUserProfileAsync(request.CreatorId, cancellationToken);

            if (senderProfile == null) {
                _logger.LogError("Sender {CreatorId} not found", request.CreatorId);
                throw new AppException($"Sender {request.CreatorId} not found", null, StatusCodes.Status404NotFound);
            }

            if (senderProfile.SubscribersCount == 0) {
                _logger.LogDebug("Sender {CreatorId} doesn't have any subscriber", senderProfile.Id);
                return Unit.Value;
            }

            var now = DateTime.UtcNow;

            var notificationMessage = new NotificationMessage {
                Type = MessageType.VideoUploaded,
                Id = request.VideoId.ToString(),
                Content = request.Title,
                ThumbnailUrl = request.ThumbnailUrl,
                Date = now,
                Sender = new NotificationMessageSender {
                    UserId = senderProfile.Id,
                    DisplayName = senderProfile.DisplayName,
                    ThumbnailUrl = senderProfile.ThumbnailUrl
                }
            };

            if (await _notificationDataAccess.HasMessageAsync(notificationMessage.Id)) {
                _logger.LogWarning("Video uploaded message {VideoId} has already been added", request.VideoId);
                return Unit.Value;
            }

            var historyExpirationTime = TimeSpan.FromDays(_config.HistoryExpirationDays);

            await _notificationDataAccess.AddMessageAsync(notificationMessage, TimeSpan.FromDays(_config.MessageExpirationDays));

            int processedCount = 0;
            while (processedCount < senderProfile.SubscribersCount) {
                var subscriptions = await _subscriptionRepository.GetSubscribersAsync(
                    senderProfile.Id,
                    false,
                    NotificationType.Enhanced,
                    SubscriptionTargetSort.SubscriptionDate,
                    new Pagination(processedCount / _config.BatchSize, _config.BatchSize),
                    cancellationToken);

                await _notificationDataAccess.AddMessageToUsersAsync(
                    subscriptions.Select(x => x.UserId).ToList(),
                    notificationMessage.Id,
                    now,
                    historyExpirationTime,
                    cancellationToken);

                processedCount += _config.BatchSize;
            }

            return Unit.Value;
        }

    }
}
