namespace Subscriptions.Domain.Models {
    public class NotificationMessage {
        public string Id { get; set; }
        public MessageType Type { get; set; }
        public NotificationMessageSender Sender { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public class NotificationMessageWithChecked : NotificationMessage {
        public bool Checked { get; set; }
    }

    public class NotificationMessageSender {
        public string? UserId { get; set; }
        public string DisplayName { get; set; }
        public string? ThumbnailUrl { get; set; }
    }

    public enum MessageType {
        VideoUploaded
    }
}
