using Application.Contracts;
using Users.API.Application.DtoModels;

namespace Users.API.Application.Commands {
    public class UpdateUserCommand : ICommand {
        public string UserId { get; set; }
        public UpdateUserProfileBasicInfo? UpdateBasicInfo { get; set; }
        public UpdateUserBrandingImages? UpdateImages { get; set; }
        public UpdateUserChannelLayout? UpdateLayout { get; set; }

        public UpdateUserCommand (
            string userId,
            UpdateUserProfileBasicInfo? updateBasicInfo,
            UpdateUserBrandingImages? updateImages,
            UpdateUserChannelLayout? updateLayout) {
            UserId = userId;
            UpdateBasicInfo = updateBasicInfo;
            UpdateImages = updateImages;
            UpdateLayout = updateLayout;
        }
    }
}
