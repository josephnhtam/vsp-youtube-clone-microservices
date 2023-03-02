namespace Community.API.Application.DtoModels {
    public class VideoForumCreationPropertiesDto {
        public Guid VideoId { get; set; }
        public InternalUserProfileDto CreatorProfile { get; set; }
        public bool AllowedToComment { get; set; }
    }
}
