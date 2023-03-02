using EventBus;
using VideoManager.API.Application.DtoModels;

namespace VideoManager.API.Application.IntegrationEvents.Users {
    public class UserProfileCreatedOrUpdatedIntegrationEvent : IntegrationEvent<UserProfileCreatedOrUpdatedIntegrationEventTopic> {
        public InternalUserProfileDto UserProfile { get; set; }

        public UserProfileCreatedOrUpdatedIntegrationEvent (InternalUserProfileDto userProfile) {
            UserProfile = userProfile;
        }
    }

    public class UserProfileCreatedOrUpdatedIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Users." + properties.TopicName;
        }
    }
}
