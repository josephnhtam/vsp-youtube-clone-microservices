namespace Subscriptions.API.Application.DtoModels {
    public class UserProfileDto {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
