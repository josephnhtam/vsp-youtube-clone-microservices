using Application.Contracts;

namespace Users.API.Application.Commands {
    public class CreateUserProfileCommand : ICommand {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string? ThumbnailToken { get; set; }

        public CreateUserProfileCommand (string userId, string displayName, string? thumbnailToken) {
            UserId = userId;
            DisplayName = displayName;
            ThumbnailToken = thumbnailToken;
        }
    }
}
