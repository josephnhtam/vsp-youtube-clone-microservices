namespace Library.API.Application.DtoModels {
    public class InternalUserProfileDto {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string? Handle { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long Version { get; set; }
    }
}
