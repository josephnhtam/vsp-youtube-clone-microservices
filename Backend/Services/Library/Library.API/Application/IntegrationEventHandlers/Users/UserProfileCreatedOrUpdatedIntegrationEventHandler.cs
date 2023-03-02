using EventBus;
using Library.API.Application.Commands;
using Library.API.Application.IntegrationEvents.Users;
using MediatR;

namespace Library.API.Application.IntegrationEventHandlers.Users {
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
                new CreateOrUpdateUserProfileCommand(
                    userProfile.Id,
                    userProfile.DisplayName,
                    userProfile.Handle,
                    userProfile.ThumbnailUrl,
                    userProfile.Version,
                    true));
        }

    }

    public class UserProfileCreatedOrUpdatedIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Library." + properties.QueueName;
        }
    }
}
