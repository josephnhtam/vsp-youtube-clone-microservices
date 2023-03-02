using Application.Contracts;

namespace Search.API.Application.Commands {
    public class UpdateUserProfileCommand : ICommand {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long Version { get; set; }

        public UpdateUserProfileCommand (string id, string displayName, string? handle, string? thumbnailUrl, long version) {
            Id = id;
            DisplayName = displayName;
            Handle = handle;
            ThumbnailUrl = thumbnailUrl;
            Version = version;
        }
    }
}