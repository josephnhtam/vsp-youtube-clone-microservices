using Application.Contracts;

namespace VideoStore.API.Application.Commands {
    public class CreateOrUpdateUserProfileCommand : ICommand {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long Version { get; set; }
        public bool UpdateIfExists { get; set; }

        public CreateOrUpdateUserProfileCommand (string id, string displayName, string? handle, string? thumbnailUrl, long version, bool updateIfExists) {
            Id = id;
            DisplayName = displayName;
            Handle = handle;
            ThumbnailUrl = thumbnailUrl;
            Version = version;
            UpdateIfExists = updateIfExists;
        }
    }
}
