using EventBus;
using MediatR;
using Search.API.Application.Commands;
using Search.API.Application.IntegrationEvents.Users;

namespace Search.API.Application.IntegrationEventHandlers.Users {
    public class UserProfileCreatedOrUpdatedIntegrationEventHandler :
        IntegrationEventHandler<UserProfileCreatedOrUpdatedIntegrationEvent, UserProfileCreatedOrUpdatedIntegrationEventQueue> {

        private readonly IMediator _mediator;
        private readonly ILogger<UserProfileCreatedOrUpdatedIntegrationEventHandler> _logger;

        public UserProfileCreatedOrUpdatedIntegrationEventHandler (
            IMediator mediator, ILogger<UserProfileCreatedOrUpdatedIntegrationEventHandler> logger) {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task Handle (UserProfileCreatedOrUpdatedIntegrationEvent integrationEvent,
            IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {

            var userProfile = integrationEvent.UserProfile;

            _logger.LogInformation("Obtained user profile ({UserId})", userProfile.Id);

            await _mediator.Send(
                new UpdateUserProfileCommand(
                    userProfile.Id,
                    userProfile.DisplayName,
                    userProfile.Handle,
                    userProfile.ThumbnailUrl,
                    userProfile.Version));
        }

    }

    public class UserProfileCreatedOrUpdatedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Search." + properties.QueueName;
        }
    }
}
