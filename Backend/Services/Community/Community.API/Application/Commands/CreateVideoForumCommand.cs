using Application.Contracts;
using Community.API.Application.DtoModels;

namespace Community.API.Application.Commands {
    public class CreateVideoForumCommand : ICommand {
        public Guid VideoId { get; set; }
        public InternalUserProfileDto CreatorProfile { get; set; }
        public bool AllowedToComment { get; set; }

        public CreateVideoForumCommand (Guid videoId, InternalUserProfileDto creatorProfile, bool allowedToComment) {
            VideoId = videoId;
            CreatorProfile = creatorProfile;
            AllowedToComment = allowedToComment;
        }
    }
}
