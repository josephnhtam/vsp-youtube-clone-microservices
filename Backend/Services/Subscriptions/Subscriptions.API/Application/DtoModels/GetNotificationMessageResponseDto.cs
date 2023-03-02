namespace Subscriptions.API.Application.DtoModels {
    public class GetNotificationMessageResponseDto {
        public int TotalCount { get; set; }
        public List<NotificationMessageDto> Messages { get; set; }
    }
}
