namespace Subscriptions.API.Application.Configurations {
    public class NotificationConfiguration {
        public int BatchSize { get; set; } = 1000;
        public int MessageExpirationDays { get; set; } = 60;
        public int HistoryExpirationDays { get; set; } = 30;
    }
}
