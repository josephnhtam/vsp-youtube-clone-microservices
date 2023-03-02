using Application.Contracts;
using History.API.Application.DtoModels;
using History.Domain.Models;

namespace History.API.Application.Commands {
    public class RegisterVideoCommand : ICommand {
        public Guid VideoId { get; set; }
        public InternalUserProfileDto CreatorProfile { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public VideoVisibility Visibility { get; set; }
        public DateTimeOffset CreateDate { get; set; }

        public RegisterVideoCommand (Guid videoId, InternalUserProfileDto creatorProfile, string title, string description, string tags, VideoVisibility visibility, DateTimeOffset createDate) {
            VideoId = videoId;
            CreatorProfile = creatorProfile;
            Title = title;
            Description = description;
            Tags = tags;
            Visibility = visibility;
            CreateDate = createDate;
        }
    }
}
