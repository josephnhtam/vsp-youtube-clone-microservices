using EventBus;
using Users.API.Application.DtoModels;

namespace Users.API.IntegrationEvents {
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
