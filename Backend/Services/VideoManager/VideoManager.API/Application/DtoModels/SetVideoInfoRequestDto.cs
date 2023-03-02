using VideoManager.Domain.Models;

namespace VideoManager.API.Application.DtoModels {
    public class SetVideoInfoRequestDto {
        public Guid VideoId { get; set; }
        public SetVideoBasicInfoRequestDto? SetBasicInfo { get; set; }
        public SetVideoVisibilityInfoRequestDto? SetVisibilityInfo { get; set; }
    }

    public class SetVideoBasicInfoRequestDto {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public Guid? ThumbnailId { get; set; }
    }

    public class SetVideoVisibilityInfoRequestDto {
        public VideoVisibility Visibility { get; set; }
    }
}
