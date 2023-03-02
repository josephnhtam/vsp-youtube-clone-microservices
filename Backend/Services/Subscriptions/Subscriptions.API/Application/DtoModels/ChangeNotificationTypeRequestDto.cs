
using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.DtoModels {
    public class ChangeNotificationTypeRequestDto {
        public string UserId { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
