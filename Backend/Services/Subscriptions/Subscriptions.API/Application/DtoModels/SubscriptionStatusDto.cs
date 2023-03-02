using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.DtoModels {
    public class SubscriptionStatusDto {
        public long SubscribersCount { get; set; }
        public bool IsSubscribed { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
