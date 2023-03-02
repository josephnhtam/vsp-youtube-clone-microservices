using Users.Domain.Models;

namespace Users.API.Application.DtoModels {
    public class PrivateUserProfileDto {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public UserProfileStatus Status { get; set; }
    }
}
