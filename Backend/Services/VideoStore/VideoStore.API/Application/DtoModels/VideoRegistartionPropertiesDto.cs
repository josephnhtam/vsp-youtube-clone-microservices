using VideoStore.Domain.Models;

namespace VideoStore.API.Application.DtoModels {
    public class VideoRegistartionPropertiesDto {
        public Guid VideoId { get; set; }
        public InternalUserProfileDto CreatorProfile { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public VideoVisibility Visibility { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
