using Application.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Queries {
    public class GetUserProfileQuery : IQuery<UserProfile> {
        public string? UserId { get; set; }
        public string? Handle { get; set; }
        public bool ThrowIfNotFound { get; set; }

        public GetUserProfileQuery (string? userId, string? handle, bool throwIfNotFound = true) {
            UserId = userId;
            Handle = handle;
            ThrowIfNotFound = throwIfNotFound;
        }
    }
}
