using Application.Contracts;
using VideoManager.Domain.Models;

namespace VideoManager.API.Commands {
    public class SetVideoInfoCommand : ICommand<Video?> {
        public string CreatorId { get; set; }
        public Guid VideoId { get; set; }
        public SetVideoBasicInfoCommand? SetVideoBasicInfo { get; set; }
        public SetVideoVisibilityInfoCommand? SetVideoVisibilityInfo { get; set; }
    }

    public class SetVideoBasicInfoCommand {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public Guid? ThumbnailId { get; set; }
    }

    public class SetVideoVisibilityInfoCommand {
        public VideoVisibility Visibility { get; set; }
    }
}
