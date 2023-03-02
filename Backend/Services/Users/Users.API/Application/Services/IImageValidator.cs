
using Users.Domain.Models;

namespace Users.API.Application.Services {
    public interface IImageValidator {
        ImageFile ValidateImageToken (string token, string? validCategory, string validUserId);
    }
}
