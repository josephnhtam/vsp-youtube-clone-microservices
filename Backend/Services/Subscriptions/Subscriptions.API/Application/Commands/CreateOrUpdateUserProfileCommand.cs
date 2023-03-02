using Application.Contracts;

namespace Subscriptions.API.Application.Commands {
    public class CreateOrUpdateUserProfileCommand : ICommand {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long Version { get; set; }
        public bool UpdateIfExists { get; set; }

        public CreateOrUpdateUserProfileCommand (string id, string displayName, string description, string? handle, string? thumbnailUrl, long version, bool updateIfExists) {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Handle = handle;
            ThumbnailUrl = thumbnailUrl;
            Version = version;
            UpdateIfExists = updateIfExists;
        }
    }
}
