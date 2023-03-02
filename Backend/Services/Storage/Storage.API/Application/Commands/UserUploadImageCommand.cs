using Application.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using Storage.API.Application.DtoModels;

namespace Storage.API.Application.Commands {
    public class UserUploadImageCommand : ICommand<ImageUploadResponseDto> {
        public string UserId { get; set; }
        public string Token { get; set; }
        public MultipartSection ImageSection { get; set; }

        public UserUploadImageCommand (string userId, string token, MultipartSection imageSection) {
            UserId = userId;
            Token = token;
            ImageSection = imageSection;
        }
    }
}
