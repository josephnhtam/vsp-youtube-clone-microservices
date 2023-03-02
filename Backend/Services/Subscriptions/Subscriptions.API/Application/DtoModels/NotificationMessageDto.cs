using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.DtoModels {
    public class NotificationMessageDto {
        public Guid Id { get; set; }
        public MessageType Type { get; set; }
        public NotificationMessageSenderDto Sender { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Content { get; set; }
        public bool Checked { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public class NotificationMessageSenderDto {
        public string? UserId { get; set; }
        public string DisplayName { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
