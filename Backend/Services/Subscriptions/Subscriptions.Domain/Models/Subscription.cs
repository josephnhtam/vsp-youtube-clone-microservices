using Domain;

namespace Subscriptions.Domain.Models {
    public class Subscription : ValueObject {

        public string UserId { get; private set; }
        public string TargetId { get; private set; }
        public UserProfile User { get; private set; }
        public UserProfile Target { get; private set; }
        public NotificationType NotificationType { get; private set; }
        public DateTimeOffset SubscriptionDate { get; private set; }

        private Subscription () { }

        private Subscription (string userId, string targetId, NotificationType notificationType, DateTimeOffset subscriptionDate) {
            UserId = userId;
            TargetId = targetId;
            NotificationType = notificationType;
            SubscriptionDate = subscriptionDate;
        }

        public static Subscription Create (string userId, string targetId, NotificationType notificationType, DateTimeOffset subscriptionDate) {
            return new Subscription(userId, targetId, notificationType, subscriptionDate);
        }

        public void SetNotificationType (NotificationType notificationType) {
            NotificationType = notificationType;
        }

    }
}
