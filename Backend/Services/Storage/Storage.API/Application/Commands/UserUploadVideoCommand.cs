using Application.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using Storage.Domain.Models;

namespace Storage.API.Application.Commands {
    public class UserUploadVideoCommand : ICommand<StoredFile> {
        public string UserId { get; set; }
        public string Token { get; set; }
        public MultipartSection VideoSection { get; set; }

        public UserUploadVideoCommand (string userId, string token, MultipartSection videoSection) {
            UserId = userId;
            Token = token;
            VideoSection = videoSection;
        }
    }
}
