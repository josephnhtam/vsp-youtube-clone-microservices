using Application.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Queries {
    public class GetUserChannelQuery : IQuery<UserChannel> {
        public string? UserId { get; set; }
        public string? Handle { get; set; }
        public int? MaxSectionItemsCount { get; set; }

        public GetUserChannelQuery (string? userId, string? handle, int? maxSectionItemsCount) {
            UserId = userId;
            Handle = handle;
            MaxSectionItemsCount = maxSectionItemsCount;
        }
    }
}
