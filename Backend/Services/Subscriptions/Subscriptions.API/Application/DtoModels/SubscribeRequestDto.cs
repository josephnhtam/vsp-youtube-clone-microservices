
using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.DtoModels {
    public class SubscribeRequestDto {
        public string UserId { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
